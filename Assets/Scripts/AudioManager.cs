using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    // This value will control the master volume for the entire game (range 0 to 1)
    private AudioSource audioSource;
    private readonly string MasterVolumeParam = "MasterVolume";
    private readonly string SFXVolumeParam = "SFXVolume";
    private readonly string MusicVolumeParam = "MusicVolume";

    [Header("SFX clips")]
    [SerializeField] AudioClip ButtonPress;
    [SerializeField] AudioClip Pause;
    [SerializeField] AudioClip LevelStart;
    [SerializeField] AudioClip LevelEnd;

    [Header("Music Clips")]
    [SerializeField] AudioClip mainMenu;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Mixer")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [SerializeField] AudioMixerGroup masterMixerGroup;
    private float? _masterVolume;
    public float MasterVolume
    {
        get => GetVolume(ref _masterVolume, PlayerData.MasterVolume);
        set => SetVolume(ref _masterVolume, PlayerData.MasterVolume, MasterVolumeParam, value);
    }
    private float? _musicVolume;
    public float MusicVolume
    {
        get => GetVolume(ref _musicVolume, PlayerData.MusicVolume);
        set => SetVolume(ref _musicVolume, PlayerData.MusicVolume, MusicVolumeParam, value);
    }
    private float? _sfxVolume;
    public float SfxVolume
    {
        get => GetVolume(ref _sfxVolume, PlayerData.SFXVolume);
        set => SetVolume(ref _sfxVolume, PlayerData.SFXVolume, SFXVolumeParam, value);
    }
    private float GetVolume(ref float? cache, string key) =>
        cache ??= PlayerData.GetFloatById(key, 0.5f);

    private void SetVolume(ref float? cache, string key, string mixerParam, float value)
    {
        cache = value;
        PlayerData.SetFloatById(key, value);
        SetMixerVolume(mixerParam, value);
    }


    public bool IsInitialized { get; private set; }
    protected override void Awake()
    {
        base.Awake(); // must be first
        if (Instance != this)
            return;

        musicSource.ignoreListenerPause = true;
        sfxSource.ignoreListenerPause = true; // see if needed too
    }
    void Start()
    {
        MasterVolume = MasterVolume;
        MusicVolume = MusicVolume;
        SfxVolume = SfxVolume;
        IsInitialized = true;
    }

    public void Button() => PlaySFX(ButtonPress);


    public void PlayMainMenu() => PlayMusic(mainMenu);

    private void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    private void SetMixerVolume(string parameter, float volume)
    {
        float dB = volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
        mixer.SetFloat(parameter, dB);
        bool success = mixer.GetFloat(parameter, out float val);
        Debug.Log($"{parameter} current dB: {val} | SetFloat success? {success}");
    }
}