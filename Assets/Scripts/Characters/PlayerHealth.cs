using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Сколько секунд после поглощения удара игрок неуязвим. Защищает от нескольких взрывов в один момент")]
    public float absorbGraceTime = 0.5f;

    [Header("Визуал щита")]
    [Tooltip("ПРЕФАБ визуала щита (из папки Project). Скрипт сам создаст его дочерним объектом игрока")]
    public GameObject shieldVisualPrefab;

    [Tooltip("Положение визуала относительно игрока (чтобы сфера была над полом, а не под ним)")]
    public Vector3 shieldLocalOffset = new Vector3(0f, 0.5f, 0f);

    public bool isInvincible;

    private GameObject shieldVisual;
    private bool isDead = false;
    private bool hasShield = false;
    private float graceEndTime = -1f;

    public bool HasShield => hasShield;

    private void Awake()
    {
        if (shieldVisualPrefab == null)
        {
            Debug.LogWarning("PlayerHealth: не назначен Shield Visual Prefab, визуала щита не будет");
            return;
        }

        shieldVisual = Instantiate(shieldVisualPrefab, transform);
        shieldVisual.transform.localPosition = shieldLocalOffset;
        shieldVisual.SetActive(hasShield);
    }

    public void GiveShield()
    {
        hasShield = true;
        if (shieldVisual != null)
            shieldVisual.SetActive(true);
    }

    // Пытается поглотить смертельный удар. true — удар поглощён, игрок жив
    public bool TryAbsorbHit()
    {
        if (isDead) return false;

        // Короткая неуязвимость после поглощения: второй взрыв в тот же момент не убьёт
        if (Time.time < graceEndTime) return true;

        if (!hasShield) return false;

        hasShield = false;
        graceEndTime = Time.time + absorbGraceTime;
        AudioManager.Instance?.PlayShieldAbsorb();
        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        Debug.Log("Щит поглотил удар");
        return true;
    }

    public void Die()
    {
        if (isInvincible || isDead) return;
        if (TryAbsorbHit()) return;

        isDead = true;

        Debug.Log("Player died");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied();
        }

        Destroy(gameObject);
    }
}
