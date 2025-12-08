using System;
using UnityEngine;

[DefaultExecutionOrder(-10)] // initialize early
public class LanguageService : MonoBehaviour
{
    public static LanguageService Instance { get; private set; }
    public const string PlayerPrefsKey = "app.language";

    public Language CurrentLanguage { get; private set; } = Language.English;
    public event Action<Language> OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadLanguage();
    }

    private void LoadLanguage()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            CurrentLanguage = (Language)PlayerPrefs.GetInt(PlayerPrefsKey, (int)Language.English);
        }
        else
        {
            // sensible default from system
            CurrentLanguage = Application.systemLanguage == SystemLanguage.French
                ? Language.French
                : Language.English;

            PlayerPrefs.SetInt(PlayerPrefsKey, (int)CurrentLanguage);
            PlayerPrefs.Save();
        }
    }

    public void SetLanguage(Language lang)
    {
        if (CurrentLanguage == lang) return;
        CurrentLanguage = lang;
        PlayerPrefs.SetInt(PlayerPrefsKey, (int)lang);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke(lang);
    }

    public void ToggleLanguage()
    {
        SetLanguage(CurrentLanguage == Language.English ? Language.French : Language.English);
    }
}
