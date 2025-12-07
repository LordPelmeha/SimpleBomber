using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead = false;

    public bool isInvincible;

    public void Die()
    {
        if (isInvincible || isDead) return;
        isDead = true;

        Debug.Log("Player died");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }

        Destroy(gameObject);
    }
}