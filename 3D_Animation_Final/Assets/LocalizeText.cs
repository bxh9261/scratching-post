using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizeText : MonoBehaviour
{
    [SerializeField] private LocalizationTable table;
    [SerializeField] private string key;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        UpdateText();
        if (LanguageService.Instance != null)
            LanguageService.Instance.OnLanguageChanged += HandleLanguageChanged;
    }

    private void OnDisable()
    {
        if (LanguageService.Instance != null)
            LanguageService.Instance.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(Language _)
    {
        UpdateText();
    }

    [ContextMenu("Update Text")]
    public void UpdateText()
    {
        if (table == null || string.IsNullOrEmpty(key)) return;
        _text.text = table.Get(key);
    }
}
