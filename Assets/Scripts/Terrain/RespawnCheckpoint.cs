using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawn playerRespawn =
            other.GetComponentInParent<PlayerRespawn>();

        if (playerRespawn == null)
            return;

        Vector3 newRespawnPosition;

        if (respawnPoint != null)
            newRespawnPosition = respawnPoint.position;
        else
            newRespawnPosition = transform.position;

        playerRespawn.SetRespawnPoint(newRespawnPosition);

        Debug.Log("Checkpoint activated: " + gameObject.name);
    }
}