using UnityEngine;

// Бонус «Щит»: даёт игроку защиту от одного смертельного удара
public class ShieldPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.GiveShield();
        AudioManager.Instance?.PlayShieldPickup();
        Destroy(gameObject);
    }
}
