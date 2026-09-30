// Connects the current player to the debug menu without editing gameplay scripts.
// Add future stamina, movement or enemy registrations here, or in their own scripts.
using System.Collections.Generic;
using UnityEngine;

namespace AscendJigoku.Debugging
{
    public sealed class DebugMenuGameBindings : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private readonly HashSet<PlayerHealth> players = new HashSet<PlayerHealth>();
        private float nextScan;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            var host = new GameObject("Debug Menu Bindings");
            DontDestroyOnLoad(host);
            host.AddComponent<DebugMenuGameBindings>();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextScan) return;
            nextScan = Time.unscaledTime + 1f;
            players.RemoveWhere(player => {
                if (player != null && player.isActiveAndEnabled) return false;
                DebugMenu.RemoveAll(player);
                return true;
            });
            foreach (PlayerHealth player in FindObjectsByType<PlayerHealth>())
            {
                if (!player.isActiveAndEnabled || !players.Add(player)) continue;
                DebugMenu.Watch(player, "Player", "Health", () => player.CurrentHealth + "/" + player.MaxHealth);
                DebugMenu.Watch(player, "Movement", "Position", () => player.transform.position.ToString("F2"));
                Rigidbody2D body = player.GetComponent<Rigidbody2D>();
                if (body != null)
                    DebugMenu.Watch(player, "Movement", "Velocity", () => body == null ? "Unavailable" : body.linearVelocity.ToString("F2"));
                DebugMenu.Action(player, "Player", "Take 1 damage", 1, () => player.TakeDamage(1, false));
                PlayerRespawn respawn = player.GetComponent<PlayerRespawn>();
                if (respawn != null)
                    DebugMenu.Action(player, "Player", "Respawn", 2, () => {
                        if (respawn != null) respawn.Respawn();
                    });
            }
        }

        private void OnDestroy()
        {
            foreach (PlayerHealth player in players) DebugMenu.RemoveAll(player);
        }
#endif
    }
}
