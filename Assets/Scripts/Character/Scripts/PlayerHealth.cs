using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        // If the player has no health left, restart the current scene.
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // If the player is still alive, respawn at the latest checkpoint.
        if (respawnAfterDamage && playerRespawn != null)
        {
            playerRespawn.Respawn();
        }
    }

    private void Die()
    {
        Debug.Log("Player died. Restarting scene.");

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}