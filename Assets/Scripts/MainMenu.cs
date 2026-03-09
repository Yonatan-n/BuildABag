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
    [SerializeField] GameObject masterVolume;
    [SerializeField] GameObject musicVolume;
    [SerializeField] GameObject sfxVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Play.onClick.AddListener(PlayHandler);
        Options.onClick.AddListener(OptionsHandler);
        Quit.onClick.AddListener(QuitHandler);
        CloseOptions.onClick.AddListener(CloseOptionsHandler);
        var masterKnob = masterVolume.GetComponentInChildren<Slider>();
        var musicKnob = musicVolume.GetComponentInChildren<Slider>();
        var sfxKnob = sfxVolume.GetComponentInChildren<Slider>();
        masterKnob.value = AudioManager.Instance.MasterVolume;
        musicKnob.value = AudioManager.Instance.MusicVolume;
        sfxKnob.value = AudioManager.Instance.SfxVolume;
        masterKnob.onValueChanged.AddListener(v => AudioManager.Instance.MasterVolume = v);
        musicKnob.onValueChanged.AddListener(v => AudioManager.Instance.MusicVolume = v);
        sfxKnob.onValueChanged.AddListener(v => AudioManager.Instance.SfxVolume = v);

        sfxKnob.GetComponent<SliderPointerUp>().onPointerUp.AddListener(() => AudioManager.Instance.Button());
        AudioManager.Instance.PlayMainMenu();
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
