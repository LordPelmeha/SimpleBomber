using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject settingsPanel;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OnPlayClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OnStartGameClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("Main");
    }

    public void OnBackClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OnQuitClicked()
    {
        AudioManager.Instance?.PlayButtonClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}