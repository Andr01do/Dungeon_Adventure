using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public GameObject checkMark;
    public GameObject lockIcon;
    public Image backgroundImage;
    public GameObject rewardIcon; 

    public void Setup(AchievementSO achievement, bool isCompleted)
    {
        iconImage.sprite = achievement.icon;

        // --- ЗМІНИ ТУТ (ЛОКАЛІЗАЦІЯ) ---
        if (LocalizationManager.Instance != null)
        {
            // Ми вважаємо, що в achievement.displayName записаний КЛЮЧ (наприклад "ach_win_name")
            nameText.text = LocalizationManager.Instance.GetTranslation(achievement.displayName);

            // А в achievement.description записаний КЛЮЧ опису (наприклад "ach_win_desc")
            descText.text = LocalizationManager.Instance.GetTranslation(achievement.description);
        }
        else
        {
            // Якщо менеджера немає, виводимо як є
            nameText.text = achievement.displayName;
            descText.text = achievement.description;
        }
        // --------------------------------

        if (rewardIcon) rewardIcon.SetActive(achievement.rewardArtifact != null);

        if (isCompleted)
        {
            nameText.color = Color.white;
            descText.color = Color.green;
            iconImage.color = Color.white;

            if (checkMark) checkMark.SetActive(true);
            if (lockIcon) lockIcon.SetActive(false);
            if (backgroundImage) backgroundImage.color = new Color(0, 0, 0, 0.5f);
        }
        else
        {
            nameText.color = Color.gray;
            descText.color = Color.gray;
            iconImage.color = Color.black; 

            if (checkMark) checkMark.SetActive(false);
            if (lockIcon) lockIcon.SetActive(true);
            if (backgroundImage) backgroundImage.color = new Color(0, 0, 0, 0.8f);
        }
    }
}