using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Tooltip("На сколько увеличить скорость (0.2 = +20%)")]
    public float speedIncreaseAmount = 0.3f;

    private void OnTriggerEnter(Collider other)
    {
        // Если коснулся игрок
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddSpeed(speedIncreaseAmount);

                // Уничтожаем бонус после сбора
                Destroy(gameObject);
            }
        }
    }
}