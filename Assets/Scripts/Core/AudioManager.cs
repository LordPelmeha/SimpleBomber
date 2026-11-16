using UnityEngine;
using UnityEngine.SceneManagement;

// Музыка и звуки всей игры.
// - Живёт между сценами (DontDestroyOnLoad). Объект можно положить и в MainMenu, и в Main:
//   дубликаты сами уничтожаются, остаётся один.
// - Музыка выбирается по имени сцены: меню -> menuMusic, игра -> gameMusic.
// - AudioSource создаются автоматически, если их не назначили в инспекторе.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Источники (можно оставить пустыми, создадутся сами)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Музыка")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;

    [Header("Звуки игры")]
    public AudioClip bombPlace;
    [Tooltip("Несколько вариантов взрыва, выбираются случайно")]
    public AudioClip[] explosions;
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip playerDeath;
    public AudioClip speedPickup;
    public AudioClip shieldPickup;
    public AudioClip shieldAbsorb;
    public AudioClip win;

    [Header("Интерфейс")]
    public AudioClip buttonClick;

    [Header("Имена сцен")]
    public string menuSceneName = "MainMenu";
    public string gameSceneName = "Main";

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    public float MusicVolume { get; private set; } = 0.6f;
    public float SfxVolume { get; private set; } = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        sfxSource.playOnAwake = false;

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, MusicVolume);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, SfxVolume);
        ApplyVolumes();

        SceneManager.sceneLoaded += OnSceneLoaded;
        PlaySceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
    }

    private void PlaySceneMusic(string sceneName)
    {
        if (sceneName == menuSceneName) PlayMusic(menuMusic);
        else if (sceneName == gameSceneName) PlayMusic(gameMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
        musicSource.UnPause(); // на случай, если музыка была на паузе
    }

    // Пауза и возобновление музыки (вызывает GameManager)
    public void SetPaused(bool paused)
    {
        if (musicSource == null) return;

        if (paused) musicSource.Pause();
        else musicSource.UnPause();
    }

    // ----- Громкость -----

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        ApplyVolumes();
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = MusicVolume;
        if (sfxSource != null) sfxSource.volume = SfxVolume;
    }

    // ----- Музыка по событиям -----

    public void PlayGameOverMusic() => PlayMusic(gameOverMusic);

    // ----- Звуки по событиям -----

    public void PlayBombPlace() => PlaySfx(bombPlace);
    public void PlayExplosion() => PlaySfx(PickRandom(explosions), 0.1f);
    public void PlayEnemyHit() => PlaySfx(enemyHit);
    public void PlayEnemyDeath() => PlaySfx(enemyDeath);
    public void PlayPlayerDeath() => PlaySfx(playerDeath);
    public void PlaySpeedPickup() => PlaySfx(speedPickup);
    public void PlayShieldPickup() => PlaySfx(shieldPickup);
    public void PlayShieldAbsorb() => PlaySfx(shieldAbsorb);
    public void PlayWin() => PlaySfx(win);
    public void PlayButtonClick() => PlaySfx(buttonClick);

    private void PlaySfx(AudioClip clip, float pitchJitter = 0f)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        sfxSource.PlayOneShot(clip);
    }

    private static AudioClip PickRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
