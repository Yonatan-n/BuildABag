using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    // This value will control the master volume for the entire game (range 0 to 1)
    private AudioSource audioSource;
    private readonly string MixerMasterVolume = "MasterVolume";
    [SerializeField] AudioClip ButtonPress;
    [SerializeField] AudioClip Pause;
    [SerializeField] AudioClip LevelStart;
    [SerializeField] AudioClip LevelEnd;

    [SerializeField] AudioMixer mixer;


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
        IsInitialized = true; // last line
    }

    public void Button()
    {
        audioSource.PlayOneShot(ButtonPress);
    }

    public void SetMasterVolume(float? volume)
    {
        volume ??= PlayerData.GetFloatById(PlayerData.MasterVolume, 0.5f);
        var masterVolume = (float)volume;
        float dB = masterVolume <= 0.0001f ? -80f : Mathf.Log10(masterVolume) * 20f;
        mixer.SetFloat(MixerMasterVolume, dB);
        bool success = mixer.GetFloat(MixerMasterVolume, out float val);
        Debug.Log("MasterVolume current dB: " + val + " | SetFloat success? " + success);
    }

}