using UnityEngine;

public enum AchievementType
{
    RunCondition,  
    GlobalStat    
}

[CreateAssetMenu(fileName = "New Achievement", menuName = "Game/Achievement")]
public class AchievementSO : ScriptableObject
{
    [Header("General Info")]
    public string id;                  
    public string displayName;      
    [TextArea] public string description; 
    public Sprite icon;                

    [Header("Requirements")]
    public AchievementType type;
    public string conditionID;        
    public int targetValue;            

    [Header("Reward (Optional)")]
    public ArtifactSO rewardArtifact;  
}