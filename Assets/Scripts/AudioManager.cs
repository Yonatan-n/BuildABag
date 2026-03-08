using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    // This value will control the master volume for the entire game (range 0 to 1)
    private AudioSource audioSource;
    private readonly string MasterVolume = "MasterVolume";
    private readonly string SFXVolume = "SFXVolume";
    private readonly string MusicVolume = "MusicVolume";

    [Header("SFX clips")]
    [SerializeField] AudioClip ButtonPress;
    [SerializeField] AudioClip Pause;
    [SerializeField] AudioClip LevelStart;
    [SerializeField] AudioClip LevelEnd;

    [Header("Music Clips")]
    [SerializeField] AudioClip Startup;
    [Header("Mixer")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [SerializeField] AudioMixerGroup masterMixerGroup;



    public bool IsInitialized { get; private set; }
    protected override void Awake()
    {
        base.Awake(); // must be first
        if (Instance != this)
            return;

        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }
    void Start()
    {
        SetMasterVolume(null);
        IsInitialized = true;
        StartupMusic();
    }

    public void Button()
    {
        audioSource.PlayOneShot(ButtonPress);
    }

    public void StartupMusic()
    {
        audioSource.clip = Startup;
        audioSource.Play();
    }

    public void SetMasterVolume(float? volume)
    {
        volume ??= PlayerData.GetFloatById(PlayerData.MasterVolume, 0.5f);
        SetMixerVolume(MasterVolume, (float)volume);
    }

    public void SetMusicVolume(float? volume)
    {
        volume ??= PlayerData.GetFloatById(PlayerData.MusicVolume, 0.5f);
        SetMixerVolume(MusicVolume, (float)volume);
    }

    public void SetSFXVolume(float? volume)
    {
        volume ??= PlayerData.GetFloatById(PlayerData.SFXVolume, 0.5f);
        SetMixerVolume(SFXVolume, (float)volume);
    }

    private void SetMixerVolume(string parameter, float volume)
    {
        float dB = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
        mixer.SetFloat(parameter, dB);
        bool success = mixer.GetFloat(parameter, out float val);
        Debug.Log($"{parameter} current dB: {val} | SetFloat success? {success}");
    }
}