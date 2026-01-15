using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks; 

public class GlobalProgressionManager : MonoBehaviour
{
    public static GlobalProgressionManager Instance;

    [Header("Databases")]
    public List<ArtifactSO> allArtifactsDatabase;
    public List<AchievementSO> allAchievementsDatabase;

    [Header("Debug Info")]
    public SaveData saveData;

    private string saveFilePath;
    private bool isSaving = false; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);


            saveFilePath = Path.Combine(Application.persistentDataPath, "game_save.json");
            Debug.Log($"Шлях до сейву: {saveFilePath}");
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SaveProgress()
    {
        if (isSaving) return; 


        _ = SaveProgressAsync();
    }

    private async Task SaveProgressAsync()
    {
        isSaving = true;

        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            await File.WriteAllTextAsync(saveFilePath, json);

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Помилка збереження гри: {e.Message}");
        }
        finally
        {
            isSaving = false;
        }
    }

    public void LoadProgress()
    {

        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                saveData = JsonUtility.FromJson<SaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Помилка читання сейву (файл пошкоджено?): {e.Message}");
                saveData = new SaveData(); 
            }
        }
        else
        {
            saveData = new SaveData();
        }


        ValidateDefaultArtifacts();
    }

    private void ValidateDefaultArtifacts()
    {
        foreach (var art in allArtifactsDatabase)
        {
            if (art.unlockType == ArtifactUnlockType.Default)
                UnlockArtifact(art.artifactID, false); 
        }
    }

    public bool IsAchievementCompleted(string id)
    {
        return saveData.completedAchievements.Contains(id);
    }

    public bool IsArtifactUnlocked(string id)
    {
        return saveData.unlockedArtifactIDs.Contains(id);
    }

    public void UnlockArtifact(string id, bool autoSave = true)
    {
        if (!saveData.unlockedArtifactIDs.Contains(id))
        {
            saveData.unlockedArtifactIDs.Add(id);
            if (autoSave) SaveProgress();
        }
    }

    public void UnlockDifficulty(int difficultyIndex) 
    {
        bool changed = false;
        if (difficultyIndex == 1 && !saveData.mediumDifficultyUnlocked)
        {
            saveData.mediumDifficultyUnlocked = true;
            changed = true;
            Debug.Log("MEDIUM DIFFICULTY UNLOCKED!");
        }
        else if (difficultyIndex == 2 && !saveData.hardDifficultyUnlocked)
        {
            saveData.hardDifficultyUnlocked = true;
            changed = true;
            Debug.Log("HARD DIFFICULTY UNLOCKED!");
        }

        if (changed) SaveProgress();
    }


    public string CheckAchievements(List<string> metRunConditions)
    {
        string log = "";
        bool needsSave = false;
        foreach (var ach in allAchievementsDatabase)
        {
            if (IsAchievementCompleted(ach.id)) continue;
            bool unlocked = false;
            if (ach.type == AchievementType.GlobalStat)
            {
                if (ach.conditionID == "total_kills" && saveData.totalEnemiesKilled >= ach.targetValue) unlocked = true;
                else if (ach.conditionID == "total_runs" && saveData.totalRunsPlayed >= ach.targetValue) unlocked = true;
                else if (ach.conditionID == "boss_kills" && saveData.bossesDefeated >= ach.targetValue) unlocked = true;
            }
            else if (ach.type == AchievementType.RunCondition)
            {
                if (metRunConditions.Contains(ach.conditionID)) unlocked = true;
            }
            if (unlocked)
            {
                saveData.completedAchievements.Add(ach.id);
                log += $"\n<color=yellow>ACHIEVEMENT:</color> {ach.displayName}";

                if (ach.rewardArtifact != null)
                {
                    UnlockArtifact(ach.rewardArtifact.artifactID, false);
                    log += $" (+ {ach.rewardArtifact.artifactName})";
                }
                needsSave = true;
            }
        }
        if (needsSave) SaveProgress();
        return log;
    }
    [ContextMenu("Delete Save File")]
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
            LoadProgress(); 
        }
    }
    public void SaveActiveRun(int floor, int difficulty, PlayerRunData playerStats, List<ArtifactSO> artifacts)
    {
        ActiveRunData runData = new ActiveRunData();
        runData.floorNumber = floor;
        runData.difficultyIndex = difficulty;
        runData.playerData = playerStats;
        foreach (var art in artifacts)
        {
            if (art != null) runData.equippedArtifactIDs.Add(art.artifactID);
        }

        saveData.activeRun = runData;
        SaveProgress();
        Debug.Log($"Забіг збережено на поверсі {floor}");
    }
    public void DeleteActiveRun()
    {
        if (saveData.activeRun != null)
        {
            saveData.activeRun = null;
            SaveProgress();
            Debug.Log("Активний забіг видалено (смерть або фінал).");
        }
    }

    public bool HasActiveRun()
    {
        return saveData.activeRun != null && saveData.activeRun.floorNumber > 0;
    }
    public List<ArtifactSO> GetArtifactsFromIDs(List<string> ids)
    {
        List<ArtifactSO> foundArtifacts = new List<ArtifactSO>();
        foreach (string id in ids)
        {
            ArtifactSO art = allArtifactsDatabase.Find(x => x.artifactID == id);
            if (art != null) foundArtifacts.Add(art);
        }
        return foundArtifacts;
    }
    public void ResetGlobalProgress()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        saveData = new SaveData();
        ValidateDefaultArtifacts();
        SaveProgress();
        Debug.Log("ПРОГРЕС ПОВНІСТЮ СИНУТО!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}