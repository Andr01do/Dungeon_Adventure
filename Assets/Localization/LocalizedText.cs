using UnityEngine;
using TMPro; // Обов'язково для TextMeshPro

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    public string key; // Ключ, наприклад: btn_play

    private TextMeshProUGUI textComponent;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        // Підписуємося на подію зміни мови
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;

        // Оновлюємо текст одразу при старті
        UpdateText();
    }

    private void OnDestroy()
    {
        // Обов'язково відписуємося, щоб не було помилок при знищенні об'єкта
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    private void UpdateText()
    {
        // Беремо переклад з менеджера
        textComponent.text = LocalizationManager.Instance.GetTranslation(key);
    }
}