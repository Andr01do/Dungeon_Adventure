using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    // Впишіть сюди код мови в інспекторі: "en" або "ua"
    public string languageCode;

    public void ChangeLanguage()
    {
        // ВАЖЛИВО: Ми звертаємося до .Instance, а не до конкретного об'єкта
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(languageCode);
        }
    }
}