using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [Header("Pause Panel")]
    [SerializeField] Button Pause;
    [SerializeField] GameObject PausePanel;
    [SerializeField] Button BackToMainMenu;
    [SerializeField] Button Resume;

    // [Header("ColorPicker")]
    // [SerializeField] ColorPicker colorPicker;
    void Start()
    {
        Pause.onClick.AddListener(OpenPausePanel);
        BackToMainMenu.onClick.AddListener(BackToMainMenuHandler);
        Resume.onClick.AddListener(ClosePausePanel);
    }
    void OpenPausePanel()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }
    void ClosePausePanel()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    void BackToMainMenuHandler()
    {
        ClosePausePanel();
        MainMenu.GoToMainMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
