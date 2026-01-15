using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public HUDManager hudManager;
    public LevelUpUI levelUpUIInstance;
    public EndGameUI endGameUI;
    public List<CharacterStatsSO> allCharactersData;
    public int currentFloor => RunManager.Instance.currentFloor;
    public int maxFloors => RunManager.Instance.maxFloors;
    public bool isRunActive => RunManager.Instance.isRunActive;
    public Difficulty currentDifficulty => RunManager.Instance.currentDifficulty;
    public List<ArtifactSO> equippedArtifacts => RunManager.Instance.equippedArtifacts;

    public static List<ArtifactSO> transferArtifactsBuffer
    {
        get => RunManager.transferArtifactsBuffer;
        set => RunManager.transferArtifactsBuffer = value;
    }

    public enum Difficulty { Easy, Medium, Hard }

    private string levelUnlocksLog = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (RunManager.Instance == null)
        {
            GameObject runMgrObj = new GameObject("RunManager");
            runMgrObj.AddComponent<RunManager>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerStatsManager player = FindFirstObjectByType<PlayerStatsManager>();
        if (player != null) player.OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerStatsManager player = FindFirstObjectByType<PlayerStatsManager>();
        if (player != null) player.OnDeath -= OnPlayerDeath;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshReferencesInNewScene();

        PlayerStatsManager player = FindFirstObjectByType<PlayerStatsManager>();
        if (player != null) player.OnDeath += OnPlayerDeath;
    }

    public void RefreshReferencesInNewScene()
    {
        hudManager = FindFirstObjectByType<HUDManager>();
        endGameUI = FindFirstObjectByType<EndGameUI>(FindObjectsInactive.Include);
        levelUpUIInstance = FindFirstObjectByType<LevelUpUI>(FindObjectsInactive.Include);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (RunManager.Instance.isRunActive && RunManager.Instance.isTimerRunning && !RunManager.Instance.isBossDead)
        {
            if (hudManager != null) hudManager.UpdateTimerUI(RunManager.Instance.currentLevelTimer);
        }
    }


    public void StartNewRun()
    {

        GlobalProgressionManager.Instance.DeleteActiveRun();

        RunManager.Instance.savedPlayerData = null;

        int diffIndex = PlayerPrefs.GetInt("SelectedDifficulty", 0);
        RunManager.Instance.StartRun(transferArtifactsBuffer, diffIndex);

        GlobalProgressionManager.Instance.saveData.totalRunsPlayed++;
        GlobalProgressionManager.Instance.SaveProgress();

        LoadLevelScene();
    }

    public void LoadNextLevel()
    {

        PlayerStatsManager playerStats = FindFirstObjectByType<PlayerStatsManager>();
        if (playerStats != null)
        {
            RunManager.Instance.savedPlayerData = playerStats.GetSaveData();
        }

        RunManager.Instance.NextFloor();

        if (GlobalProgressionManager.Instance != null && RunManager.Instance.savedPlayerData != null)
        {
            GlobalProgressionManager.Instance.SaveActiveRun(
                RunManager.Instance.currentFloor,
                (int)RunManager.Instance.currentDifficulty,
                RunManager.Instance.savedPlayerData,
                RunManager.Instance.equippedArtifacts
            );
        }

        LoadLevelScene();
    }

    private void LoadLevelScene()
    {
        Time.timeScale = 1f;
        RunManager.Instance.ResetRunData();
        levelUnlocksLog = "";
        SceneManager.LoadScene("Game");
    }

    private void OnPlayerDeath()
    {

        GlobalProgressionManager.Instance.DeleteActiveRun();
        Debug.Log("Гравець помер. Сейв видалено.");

    }

    public void SpawnPlayerAt(Transform point)
    {
        RefreshReferencesInNewScene();
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);


        if (selectedIndex >= allCharactersData.Count) selectedIndex = 0;

        CharacterStatsSO selectedCharData = allCharactersData[selectedIndex];
        GameObject playerObj = Instantiate(selectedCharData.characterPrefab, point.position, point.rotation);

        PlayerController pc = playerObj.GetComponent<PlayerController>();
        if (pc != null)
        {

            if (RunManager.Instance.currentFloor > 1 && RunManager.Instance.savedPlayerData != null)
            {

                pc.stats.LoadFromSaveData(selectedCharData, RunManager.Instance.savedPlayerData);
            }
            else
            {

                pc.Initialize(selectedCharData);
            }

            if (levelUpUIInstance != null)
            {
                pc.levelUpScreen = levelUpUIInstance;
                levelUpUIInstance.Initialize(pc);
                levelUpUIInstance.gameObject.SetActive(false);
            }
            if (hudManager != null)
            {
                pc.healthBar = hudManager.healthBar;
                pc.staminaBar = hudManager.staminaBar;
                pc.levelText = hudManager.levelText;
                pc.xpSlider = hudManager.xpSlider;
                pc.UpdateUI();
                hudManager.UpdateFloorText(currentFloor, maxFloors);
            }

            pc.stats.OnDeath += OnPlayerDeath;
        }
    }

    public void SetTotalEnemies(int amount)
    {
        RunManager.Instance.totalEnemiesInRun = amount;
        RunManager.Instance.enemiesKilledInRun = 0;
    }

    public void RegisterEnemyKill()
    {
        RunManager.Instance.enemiesKilledInRun++;
    }

    public void OnBossDefeated()
    {
        RunManager.Instance.isBossDead = true;
        if (hudManager != null && hudManager.bossHPbar != null)
            hudManager.bossHPbar.gameObject.SetActive(false);

        GlobalProgressionManager.Instance.saveData.bossesDefeated++;
        TryDropBossArtifact();
        GlobalProgressionManager.Instance.SaveProgress();
    }

    private void TryDropBossArtifact()
    {
        List<ArtifactSO> potentialDrops = GlobalProgressionManager.Instance.allArtifactsDatabase
            .FindAll(a => a.unlockType == ArtifactUnlockType.BossDrop && !GlobalProgressionManager.Instance.IsArtifactUnlocked(a.artifactID));

        if (potentialDrops.Count == 0) return;

        ArtifactSO candidate = potentialDrops[Random.Range(0, potentialDrops.Count)];

        if (Random.Range(0f, 100f) <= candidate.dropChance)
        {
            GlobalProgressionManager.Instance.UnlockArtifact(candidate.artifactID);
            string prefix = "BOSS DROP";
            string artName = candidate.artifactName;

            if (LocalizationManager.Instance != null)
            {
                prefix = LocalizationManager.Instance.GetTranslation("msg_boss_drop");
                artName = LocalizationManager.Instance.GetTranslation(candidate.artifactName);
            }

            levelUnlocksLog += $"\n<color=red>{prefix}:</color> {artName}";
        }
    }

    public void FinishLevelLogic()
    {
        RunManager.Instance.CompleteLevel();

        float percentage = 0f;
        int total = RunManager.Instance.totalEnemiesInRun;
        int killed = RunManager.Instance.enemiesKilledInRun;
        if (total > 0) percentage = ((float)killed / total) * 100f;

        string rank = CalculateRank(percentage);
        GlobalProgressionManager.Instance.saveData.totalEnemiesKilled += killed;
        CheckAchievements(rank);

        bool isFinalLevel = (currentFloor >= maxFloors);
        string flavor = "";

        string timeStr = FormatTime(RunManager.Instance.currentLevelTimer);

        if (isFinalLevel)
        {
            CheckDifficultyUnlock();
            float totalTime = 0f;
            foreach (var t in RunManager.Instance.levelCompletionTimes) totalTime += t;

            string totalTimeStr = FormatTime(totalTime);

            if (LocalizationManager.Instance != null)
            {
                string format = LocalizationManager.Instance.GetTranslation("msg_win_final");
                flavor = string.Format(format, totalTimeStr);
            }
            else
            {
                flavor = $"VICTORY!\nTotal Time: {totalTimeStr}\n\n";
            }

            GlobalProgressionManager.Instance.DeleteActiveRun();
        }
        else
        {

            if (LocalizationManager.Instance != null)
            {
                string format = LocalizationManager.Instance.GetTranslation("msg_floor_complete");
                flavor = string.Format(format, currentFloor, timeStr);
            }
            else
            {
                flavor = $"Floor {currentFloor} cleared!\nTime: {timeStr}";
            }
        }

        GlobalProgressionManager.Instance.SaveProgress();

        if (endGameUI != null)
        {
            endGameUI.ShowWinScreen(rank, flavor, Mathf.RoundToInt(percentage), killed, total, isFinalLevel, levelUnlocksLog);
        }
    }

    public float GetTotalXPMultiplier()
    {
        float difficultyMult = RunManager.Instance.GetDifficultyXPMultiplier();
        float floorBonus = 1f + ((currentFloor - 1) * 0.15f);
        float artifactMult = CalculateStat(1f, StatType.ExperienceGain);
        return difficultyMult * artifactMult * floorBonus;
    }

    public float GetHealthMultiplier() => RunManager.Instance.GetDifficultyHealthMultiplier();

    public float CalculateStat(float baseValue, StatType type)
    {
        float percentAdd = 0f;
        float flatAdd = 0f;
        foreach (var art in RunManager.Instance.equippedArtifacts)
        {
            if (art == null) continue;
            foreach (var mod in art.modifiers)
            {
                if (mod.statType == type)
                {
                    if (mod.mode == ModifierMode.PercentAdd) percentAdd += mod.value;
                    else if (mod.mode == ModifierMode.FlatAdd) flatAdd += mod.value;
                }
            }
        }
        return (baseValue * (1f + percentAdd)) + flatAdd;
    }

    public void ShowBossHPBar(GameObject boss)
    {
        if (hudManager != null && hudManager.bossHPbar != null)
        {
            hudManager.bossHPbar.gameObject.SetActive(true);
            var uiScript = hudManager.bossHPbar.GetComponent<BossHealthUI>();
            var bossCtrl = boss.GetComponent<BossGolem>();
            if (uiScript != null && bossCtrl != null) uiScript.Init(bossCtrl);
        }
    }


    public void RestartGameFull()
    {
        StartNewRun();
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        RunManager.Instance.isRunActive = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Main Menu");
    }

    private string CalculateRank(float percentage)
    {
        if (percentage >= 100f) return "S";
        if (percentage >= 90f) return "A";
        if (percentage >= 80f) return "B";
        if (percentage >= 70f) return "C";
        if (percentage >= 60f) return "D";
        if (percentage >= 50f) return "E";
        return "F";
    }

    public string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void CheckAchievements(string rank)
    {
        List<string> currentRunConditions = new List<string>();
        if (rank == "S") currentRunConditions.Add("rank_s");
        if (rank == "A") currentRunConditions.Add("rank_a");
        if (rank == "S" && currentFloor >= 5) currentRunConditions.Add("rank_s_run");
        if (currentFloor >= maxFloors)
        {
            currentRunConditions.Add("finish_run");

            if (currentDifficulty == Difficulty.Easy) currentRunConditions.Add("beat_easy");
            if (currentDifficulty == Difficulty.Medium) currentRunConditions.Add("beat_medium");
            if (currentDifficulty == Difficulty.Hard)
            {
                currentRunConditions.Add("beat_hard");
                int charIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
                currentRunConditions.Add($"beat_hard_char_{charIndex}");
            }
        }

        string unlocksLogStr = GlobalProgressionManager.Instance.CheckAchievements(currentRunConditions);
        levelUnlocksLog += unlocksLogStr;
    }

    private void CheckDifficultyUnlock()
    {
        if (currentDifficulty == Difficulty.Easy && !GlobalProgressionManager.Instance.saveData.mediumDifficultyUnlocked)
        {
            GlobalProgressionManager.Instance.UnlockDifficulty(1);
            levelUnlocksLog += "\n<color=green>NEW MODE:</color> Medium Difficulty!";
        }
        else if (currentDifficulty == Difficulty.Medium && !GlobalProgressionManager.Instance.saveData.hardDifficultyUnlocked)
        {
            GlobalProgressionManager.Instance.UnlockDifficulty(2);
            levelUnlocksLog += "\n<color=red>NEW MODE:</color> Hard Difficulty!";
        }
    }
}