using TMPro;
using UnityEngine;

public class LanguageToggleButton : MonoBehaviour
{
    [SerializeField] private TMP_Text label; // optional, to show current language

    public void OnToggleClicked()
    {
        LanguageService.Instance?.ToggleLanguage();
        RefreshLabel();
    }

    private void Start() => RefreshLabel();

    private void RefreshLabel()
    {
        if (label == null || LanguageService.Instance == null) return;
        label.text = LanguageService.Instance.CurrentLanguage == Language.French ? "FR" : "EN";
    }
}
