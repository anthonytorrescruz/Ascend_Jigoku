using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] healthSegments;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.HealthChanged += UpdateHealthBar;

            UpdateHealthBar(
                playerHealth.CurrentHealth,
                playerHealth.MaxHealth
            );
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < healthSegments.Length; i++)
        {
            healthSegments[i].SetActive(i < currentHealth);
        }
    }
}