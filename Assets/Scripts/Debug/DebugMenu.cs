/*
 * F1: show/hide. Number keys run actions while the menu is open.
 * Starts automatically in the Editor and Development Builds; no scene setup.
 *
 * In OnEnable (using AscendJigoku.Debugging):
 *   DebugMenu.Watch(this, "Player", "Health", () => health);
 *   DebugMenu.Action(this, "Player", "Heal", 3, Heal);
 * In OnDisable:
 *   DebugMenu.RemoveAll(this);
 *
 * Use a unique action key (1-9). Getters only read values; actions call your
 * normal gameplay methods. The menu does not pause or consume gameplay input.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace AscendJigoku.Debugging
{
    public sealed class DebugMenu : MonoBehaviour
    {
        public static bool IsOpen { get; private set; }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Watch<T>(UnityEngine.Object owner, string category, string label, Func<T> getter)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            Register(new Entry { Owner = owner, Category = category, Label = label,
                Read = () => Convert.ToString(getter()) });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Action(UnityEngine.Object owner, string category, string label, int key, System.Action callback)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (key < 1 || key > 9) throw new ArgumentOutOfRangeException(nameof(key), "Use a key from 1 to 9.");
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            Register(new Entry { Owner = owner, Category = category, Label = label, Key = key, Run = callback });
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void RemoveAll(UnityEngine.Object owner)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            entries.RemoveAll(e => ReferenceEquals(e.Owner, owner));
#endif
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private sealed class Entry
        {
            public UnityEngine.Object Owner;
            public string Category, Label, Value;
            public Func<string> Read;
            public System.Action Run;
            public int Key;
        }
        private static readonly List<Entry> entries = new List<Entry>();
        private static DebugMenu instance;
        private float nextRefresh;
        private string message = "";
        private string display = "";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            entries.Clear();
            instance = null;
            IsOpen = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (instance == null) new GameObject("Debug Menu").AddComponent<DebugMenu>();
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(this); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private static void Register(Entry entry)
        {
            if (entry.Owner == null) throw new ArgumentNullException("owner");
            if (string.IsNullOrWhiteSpace(entry.Category) || string.IsNullOrWhiteSpace(entry.Label))
                throw new ArgumentException("Category and label are required.");
            entries.RemoveAll(e => e.Owner == entry.Owner && e.Category == entry.Category && e.Label == entry.Label);
            entries.Add(entry);
            entries.Sort((a, b) => string.CompareOrdinal(a.Category, b.Category));
        }

        private void Update()
        {
            if (Pressed(0)) IsOpen = !IsOpen;
            if (Time.unscaledTime >= nextRefresh)
            {
                entries.RemoveAll(e => e.Owner == null);
                if (IsOpen) RefreshDisplay();
                nextRefresh = Time.unscaledTime + 0.1f;
            }
            if (!IsOpen) return;
            for (int key = 1; key <= 9; key++)
            {
                if (!Pressed(key)) continue;
                Entry action = null;
                int matches = 0;
                foreach (Entry entry in entries)
                    if (entry.Owner != null && entry.Key == key) { action = entry; matches++; }
                if (matches > 1) { message = "Key " + key + " is assigned more than once."; continue; }
                if (action == null) continue;
                try { action.Run(); message = action.Label; }
                catch (Exception exception) { message = exception.Message; }
                RefreshDisplay();
            }
        }

        private static bool Pressed(int number)
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return false;
            return number == 0 ? keyboard.f1Key.wasPressedThisFrame
                : keyboard[(Key)((int)Key.Digit1 + number - 1)].wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(number == 0 ? KeyCode.F1 : (KeyCode)((int)KeyCode.Alpha0 + number));
#else
            return false;
#endif
        }

        private void RefreshDisplay()
        {
            var text = new System.Text.StringBuilder();
            string category = null;
            foreach (Entry entry in entries.ToArray())
            {
                if (entry.Owner == null) continue;
                if (category != entry.Category)
                {
                    category = entry.Category;
                    text.AppendLine().AppendLine(category);
                }
                if (entry.Read != null)
                {
                    try { entry.Value = entry.Read(); }
                    catch (Exception exception) { entry.Value = "Error: " + exception.Message; }
                    text.AppendLine(entry.Owner.name + " / " + entry.Label + ": " + entry.Value);
                }
                else text.AppendLine("[" + entry.Key + "] " + entry.Label + " (" + entry.Owner.name + ")");
            }
            display = text.Length > 0 ? text.ToString() : "\nNo values registered yet.";
        }

        private void OnGUI()
        {
            if (!IsOpen) return;
            var content = new GUIContent("DEBUG  [F1 hide]\n" + display + "\n" + message);
            float width = Mathf.Min(350f, Screen.width - 24f);
            float height = GUI.skin.label.CalcHeight(content, width - 20f) + 20f;
            GUI.Box(new Rect(12, 12, width, height), GUIContent.none);
            GUI.Label(new Rect(22, 20, width - 20f, height - 12f), content);
        }

        private void OnDisable() { if (instance == this) IsOpen = false; }
        private void OnDestroy() { if (instance == this) { instance = null; IsOpen = false; } }
#endif
    }
}
