using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static readonly string level1 = "level1";

    [SerializeField] Button Play;
    [SerializeField] Button Options;
    [SerializeField] Button Quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Play.onClick.AddListener(PlayHandler);
        Options.onClick.AddListener(OptionsHandler);
        Quit.onClick.AddListener(QuitHandler);

    }
    void PlayHandler()
    {
        StartLevel1();
    }
    void OptionsHandler()
    {
        Debug.Log("options");
    }
    void QuitHandler()
    {
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
