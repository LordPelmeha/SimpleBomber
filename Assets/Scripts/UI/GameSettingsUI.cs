using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsUI : MonoBehaviour
{
    [Header("Player Speed (cells/sec)")]
    public Slider playerSpeedSlider;
    public TextMeshProUGUI playerSpeedValueText;

    [Header("Enemy Speed (cells/sec)")]
    public Slider enemySpeedSlider;
    public TextMeshProUGUI enemySpeedValueText;

    [Header("Bomb Range (cells)")]
    public Slider bombRangeSlider;
    public TextMeshProUGUI bombRangeValueText;

    [Header("Enemy Spawn Interval (sec)")]
    public Slider spawnIntervalSlider;
    public TextMeshProUGUI spawnIntervalValueText;

    private void Start()
    {
        playerSpeedSlider.value = GameSettings.PlayerCellsPerSecond;
        UpdatePlayerSpeed(playerSpeedSlider.value);


        enemySpeedSlider.value = GameSettings.EnemyCellsPerSecond;
        UpdateEnemySpeed(enemySpeedSlider.value);

        bombRangeSlider.value = GameSettings.BombRange;
        UpdateBombRange(bombRangeSlider.value);

        spawnIntervalSlider.value = GameSettings.EnemySpawnInterval;
        UpdateSpawnInterval(spawnIntervalSlider.value);
    }


    public void OnPlayerSpeedSliderChanged(float value)
    {
        UpdatePlayerSpeed(value);
    }

    private void UpdatePlayerSpeed(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        GameSettings.PlayerCellsPerSecond = value;
        playerSpeedValueText.text = intValue.ToString();
    }

    public void OnEnemySpeedSliderChanged(float value)
    {
        UpdateEnemySpeed(value);
    }

    private void UpdateEnemySpeed(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        GameSettings.EnemyCellsPerSecond = value;
        enemySpeedValueText.text = intValue.ToString();
    }

    public void OnBombRangeSliderChanged(float value)
    {
        UpdateBombRange(value);
    }

    private void UpdateBombRange(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        GameSettings.BombRange = intValue;
        bombRangeValueText.text = intValue.ToString();
    }

    public void OnSpawnIntervalSliderChanged(float value)
    {
        UpdateSpawnInterval(value);
    }

    private void UpdateSpawnInterval(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        GameSettings.EnemySpawnInterval = value;
        spawnIntervalValueText.text = intValue.ToString("0") + " s";
    }
}