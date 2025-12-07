using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Реагируем только на игрока
        if (!other.CompareTag("Player"))
            return;

        // Если игра уже закончена – ничего не делаем
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerReachedExit();
        }
    }
}