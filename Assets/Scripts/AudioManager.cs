using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    private readonly string MasterVolumeParam = "MasterVolume";
    private readonly string SFXVolumeParam = "SFXVolume";
    private readonly string MusicVolumeParam = "MusicVolume";

    [Header("SFX clips")]
    [SerializeField] AudioClip ButtonPress;
    [SerializeField] AudioClip ButtonPressLong;
    [SerializeField] AudioClip ButtonDownUp;
    [SerializeField] AudioClip LevelStart;
    [SerializeField] AudioClip LevelEnd;
    [SerializeField] AudioClip Submit;
    [SerializeField] AudioClip Bell;

    [SerializeField] AudioClip CashRegister;

    [SerializeField] AudioClip AddCash;

    [SerializeField] AudioClip PenScribble;

    [SerializeField] AudioClip Swoosh;

    [SerializeField] AudioClip BubblePop;

    [SerializeField] AudioClip TimesUp;

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
        PlayMainMenu(); // loop all the time for now
    }

    public void Button() => PlaySFX(ButtonPress);
    public void PlaySubmit() => PlaySFX(Submit);
    public void PlayBubble() => PlaySFX(BubblePop);
    public void PlayLevelEnd() => PlaySFX(LevelEnd);
    public void PlayMoney() => PlaySFX(CashRegister);

    public void PlayScribble()
    {
        sfxSource.clip = PenScribble;
        sfxSource.loop = true;
        sfxSource.Play();
    }
    public void StopScribble()
    {
        sfxSource.loop = false;
        sfxSource.Stop();
    }


    public void PlayMainMenu() => PlayMusic(mainMenu);

    // not used yet
    public void StopTrack()
    {
        StopMusic();
    }

    private void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
    // not used yet
    private void StopMusic()
    {
        musicSource.Stop();
    }

    private void PlaySFX(AudioClip clip)
    {
        sfxSource.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
        sfxSource.ignoreListenerPause = true;
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