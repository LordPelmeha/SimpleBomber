using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public HUDController hud;

    private float elapsedTime = 0f;
    private int enemiesKilled = 0;

    public static GameManager Instance { get; private set; }

    public bool IsGameOver { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (IsGameOver) return;

        elapsedTime += Time.deltaTime;
        if (hud != null)
        {
            hud.SetTime(elapsedTime);
        }
    }

    public void OnPlayerDied()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        Debug.Log("GAME OVER");

        if (hud != null)
        {
            hud.ShowGameOver();
        }

        StartCoroutine(ReloadSceneCoroutine());
    }

    public void OnPlayerReachedExit()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        Debug.Log("YOU WIN!");

        if (hud != null)
        {
            hud.ShowWin();
        }

        StartCoroutine(ReloadSceneCoroutine());
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;

        if (hud != null)
        {
            hud.SetEnemiesKilled(enemiesKilled);
        }
    }

    private IEnumerator ReloadSceneCoroutine()
    {
        yield return new WaitForSeconds(2f);

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}