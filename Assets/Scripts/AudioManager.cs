using System.Collections;
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
    [SerializeField] AudioClip LevelEnd;
    [SerializeField] AudioClip Submit;
    [SerializeField] AudioClip Bell;
    [SerializeField] AudioClip CashRegister;
    [SerializeField] AudioClip AddCash;
    [SerializeField] AudioClip PenScribble;
    [SerializeField] AudioClip Swoosh;
    [SerializeField] AudioClip BubblePop;
    [SerializeField] AudioClip TimesUp;

    [Header("Voices Clips")]
    [SerializeField] AudioClip[] dislike;
    [SerializeField] AudioClip[] medium;
    [SerializeField] AudioClip[] pleased;

    [Header("Music Clips")]
    [SerializeField] AudioClip mainMenu;
    [SerializeField] AudioClip trackGoth;
    [SerializeField] AudioClip trackY2K;
    [SerializeField] AudioClip trackSportsy; // waiting on a real track, placeholder
    [SerializeField] AudioClip trackGirly; // same as menu

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
    T RandomItem<T>(T[] list) => list[Random.Range(0, list.Length)];

    public void PlayDislike()
    {
        var clip = RandomItem(dislike);
        PlaySFX(clip);
    }

    public void PlayMedium()
    {
        var clip = RandomItem(medium);
        PlaySFX(clip);
    }
    public void PlayPleased()
    {
        var clip = RandomItem(pleased);
        PlaySFX(clip);
    }


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

    private void TransitionToTrack(AudioClip newClip)
    {
        if (musicSource.clip == newClip) return; // already playing
        StartCoroutine(FadeTransition(newClip));
    }

    public void TransitionToTrackByTheme(BagTheme theme)
    {
        AudioClip clip = theme switch
        {
            BagTheme.Goth => trackGoth,
            BagTheme.Y2K => trackY2K,
            BagTheme.Girly => trackGirly,
            BagTheme.Sportsy => trackSportsy,
            _ => mainMenu
        };
        Debug.Log("track change to " + clip);
        TransitionToTrack(clip);
    }

    private IEnumerator FadeTransition(AudioClip newClip, float fadeDuration = 1.5f)
    {
        float userVolume = MusicVolume;
        yield return StartCoroutine(Fade(userVolume, 0f, fadeDuration));
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        // Ensure mixer is at silence before we start fade-in
        mixer.SetFloat(MusicVolumeParam, -80f);
        musicSource.Play();
        yield return StartCoroutine(Fade(0f, userVolume, fadeDuration));
    }

    private float volumeToDB(float volume)
    {
        return volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float linear = Mathf.Lerp(from, to, elapsed / duration);
            // Convert to dB for the mixer
            float dB = volumeToDB(linear);
            mixer.SetFloat(MusicVolumeParam, dB);
            yield return null;
        }
        // Ensure we land exactly on target
        float finaldB = volumeToDB(to);
        mixer.SetFloat(MusicVolumeParam, finaldB);
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
        float dB = volumeToDB(volume);
        mixer.SetFloat(parameter, dB);
        bool success = mixer.GetFloat(parameter, out float val);
        Debug.Log($"{parameter} current dB: {val} | SetFloat success? {success}");
    }
}