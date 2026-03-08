using UnityEngine;

public static class PlayerData
{
    // ----------- Audio ------------
    public const string MasterVolume = "MasterVolume";
    public const string MusicVolume = "MusicVolume";
    public const string SFXVolume = "SFXVolume";
    // -----------


    public static int GetIntById(string _id, int _default = 0) => PlayerPrefs.GetInt(_id, _default);
    public static void SetIntById(string id, int value, bool inc = false)
    {
        if (inc)
            value += GetIntById(id);
        PlayerPrefs.SetInt(id, value);
    }
    public static float GetFloatById(string _id, float _default = 0) => PlayerPrefs.GetFloat(_id, _default);
    public static void SetFloatById(string _id, float value) => PlayerPrefs.SetFloat(_id, value);
    public static bool GetBoolById(string id, bool defaultValue = false)
    {
        if (PlayerPrefs.HasKey(id))
            return PlayerPrefs.GetInt(id) == 1;

        PlayerPrefs.SetInt(id, defaultValue ? 1 : 0);
        return defaultValue;

    }
    public static void SetBoolById(string _id, bool value) => PlayerPrefs.SetInt(_id, value ? 1 : 0);

    // for debugging
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
