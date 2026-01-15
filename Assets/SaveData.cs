using System.Collections.Generic;

[System.Serializable]
public class SaveData
{

    public int totalRunsPlayed;
    public int totalEnemiesKilled;
    public int bossesDefeated;
    public bool mediumDifficultyUnlocked;
    public bool hardDifficultyUnlocked;

    public List<string> completedAchievements = new List<string>();
    public List<string> unlockedArtifactIDs = new List<string>();


    public ActiveRunData activeRun;
}

[System.Serializable]
public class ActiveRunData
{
    public int floorNumber;
    public int difficultyIndex;


    public List<string> equippedArtifactIDs = new List<string>();

    public PlayerRunData playerData;
}