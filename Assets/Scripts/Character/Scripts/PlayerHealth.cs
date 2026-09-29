using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private PlayerRespawn playerRespawn;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event Action<int, int> HealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount, bool respawnAfterDamage)
    {
        if (amount <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        Debug.Log("Player health: " + currentHealth + "/" + maxHealth);

        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (respawnAfterDamage && playerRespawn != null)
        {
            playerRespawn.Respawn();
        }

        // Nothing happens at 0 health yet.
        // Death behavior can be added here later.
    }
}