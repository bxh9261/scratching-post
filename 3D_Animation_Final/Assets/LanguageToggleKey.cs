using UnityEngine;

public class LanguageToggleKey : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.Z;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            LanguageService.Instance?.ToggleLanguage();
            Debug.Log("Language toggled: " + LanguageService.Instance.CurrentLanguage);
        }
    }
}
