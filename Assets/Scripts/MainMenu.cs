using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static readonly string level1 = "level1";

    [SerializeField] Button Play;
    [SerializeField] Button Options;
    [SerializeField] Button Quit;

    [Header("Options Panel")]
    [SerializeField] Button CloseOptions;
    [SerializeField] GameObject OptionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Play.onClick.AddListener(PlayHandler);
        Options.onClick.AddListener(OptionsHandler);
        Quit.onClick.AddListener(QuitHandler);
        CloseOptions.onClick.AddListener(CloseOptionsHandler);

    }

    void CloseOptionsHandler()
    {
        AudioManager.Instance.Button();
        OptionsPanel.SetActive(false);
    }
    void PlayHandler()
    {
        AudioManager.Instance.Button();
        StartLevel1();
    }
    void OptionsHandler()
    {
        AudioManager.Instance.Button();
        OptionsPanel.SetActive(true);
    }
    void QuitHandler()
    {
        AudioManager.Instance.Button();
        Application.Quit();
    }


    void StartLevel1()
    {
        LoadScene(level1);
    }

    void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    static public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
