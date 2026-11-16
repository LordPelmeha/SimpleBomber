using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ползунки громкости музыки и звуков в меню настроек.
// Вешается на объект SettingsPanel. Значения ползунков: от 0 до 1.
public class AudioSettingsUI : MonoBehaviour
{
    [Header("Музыка")]
    public Slider musicSlider;
    public TextMeshProUGUI musicValueText;

    [Header("Звуки")]
    public Slider sfxSlider;
    public TextMeshProUGUI sfxValueText;

    private void Awake()
    {
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    // Каждый раз при открытии панели берём текущую громкость
    private void OnEnable()
    {
        AudioManager audio = AudioManager.Instance;
        float music = audio != null ? audio.MusicVolume : 0.6f;
        float sfx = audio != null ? audio.SfxVolume : 0.8f;

        if (musicSlider != null) musicSlider.SetValueWithoutNotify(music);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfx);

        UpdateMusicText(music);
        UpdateSfxText(sfx);
    }

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        UpdateMusicText(value);
    }

    private void OnSfxChanged(float value)
    {
        AudioManager.Instance?.SetSfxVolume(value);
        UpdateSfxText(value);
    }

    private void UpdateMusicText(float value)
    {
        if (musicValueText != null) musicValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void UpdateSfxText(float value)
    {
        if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
