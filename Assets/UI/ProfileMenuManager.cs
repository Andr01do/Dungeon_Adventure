using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProfileMenuManager : MonoBehaviour
{
    [Header("Статистика Гравця")]
    public TextMeshProUGUI totalRunsText;
    public TextMeshProUGUI totalKillsText;
    public TextMeshProUGUI bossKillsText;

    public GameObject slotPrefab;
    public Transform gridContainer;
    private void OnEnable()
    {
        UpdateStats();
        RefreshUI();
    }

    void UpdateStats()
    {

        var data = GlobalProgressionManager.Instance.saveData;

        totalRunsText.text = data.totalRunsPlayed.ToString();
        totalKillsText.text = data.totalEnemiesKilled.ToString();
        bossKillsText.text = data.bossesDefeated.ToString();
    }

    public void RefreshUI()
    {

        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        List<AchievementSO> allAchievements = GlobalProgressionManager.Instance.allAchievementsDatabase;

        foreach (var ach in allAchievements)
        {
            GameObject newSlot = Instantiate(slotPrefab, gridContainer);
            AchievementSlotUI ui = newSlot.GetComponent<AchievementSlotUI>();

            bool unlocked = GlobalProgressionManager.Instance.IsAchievementCompleted(ach.id);

            ui.Setup(ach, unlocked);
        }
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}