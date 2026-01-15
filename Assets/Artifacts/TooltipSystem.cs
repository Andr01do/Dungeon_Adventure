using UnityEngine;
using TMPro; // Не забудь додати, якщо використовуєш TextMeshPro

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    [Header("UI Components")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;

    private void Awake()
    {
        Instance = this;
        Hide(); // Ховаємо при старті
    }

    private void Update()
    {
        // Панель слідує за мишкою
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            // Зміщення, щоб курсор не перекривав текст
            float pivotX = mousePos.x / Screen.width;
            float pivotY = mousePos.y / Screen.height;

            // Проста логіка слідування (можна покращити)
            tooltipPanel.transform.position = mousePos + new Vector2(15f, -15f);
        }
    }

    public void Show(string titleKey, string content, bool isContentKey = true) // <--- Додали isContentKey = true
    {
        string finalTitle = titleKey;
        string finalContent = content;

        if (LocalizationManager.Instance != null)
        {
            // Заголовок перекладаємо завжди
            finalTitle = LocalizationManager.Instance.GetTranslation(titleKey);

            // Опис перекладаємо, ТІЛЬКИ якщо isContentKey == true
            if (isContentKey)
            {
                finalContent = LocalizationManager.Instance.GetTranslation(content);
            }
            // Якщо isContentKey == false, то finalContent залишається таким, як прийшов (простий текст)
        }

        headerText.text = finalTitle;
        contentText.text = finalContent;

        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}