using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI statusText;

    public void SetTime(float timeSeconds)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);
        if (timeText != null)
        {
            timeText.text = $"TIME: {minutes:00}:{seconds:00}";
        }
    }

    public void SetEnemiesKilled(int count)
    {
        if (enemiesText != null)
        {
            enemiesText.text = $"KILLS: {count}";
        }
    }

    public void ShowGameOver()
    {
        if (statusText == null) return;

        statusText.gameObject.SetActive(true);
        statusText.text = "GAME OVER";
        statusText.color = Color.red;
    }

    public void ShowWin()
    {
        if (statusText == null) return;

        statusText.gameObject.SetActive(true);
        statusText.text = "YOU WIN";
        statusText.color = Color.cyan;
    }
}