using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.escapeKey.wasPressedThisFrame)
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.IsGameOver) return;

            if (gm.IsPaused)
            {
                OnContinueClicked();
            }
            else
            {
                gm.PauseGame();
                if (pausePanel != null)
                    pausePanel.SetActive(true);
            }
        }
    }


    public void OnContinueClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.ResumeGame();
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void OnRestartClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.RestartLevel();
        }
    }

    public void OnMainMenuClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.LoadMainMenu();
        }
    }

    public void OnQuitClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.QuitGame();
        }
    }
}