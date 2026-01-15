using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArtifactModifier
{
    public StatType statType;
    public ModifierMode mode;
    public float value;
}

public enum ArtifactUnlockType { Default, Achievement, BossDrop }

[CreateAssetMenu(fileName = "NewArtifact", menuName = "Game/Artifact")]
public class ArtifactSO : ScriptableObject
{
    [Header("System ID")]
    public string artifactID;
    [Header("Для меню ачівок")]
    [TextArea] public string unlockConditionDescription; 
    [Header("Unlock Settings")]
    public ArtifactUnlockType unlockType;
    public string achievementConditionID;
    [Range(0, 100)] public float dropChance = 10f;

    [Header("UI Info")]
    public string artifactName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Bonuses")]
    public List<ArtifactModifier> modifiers;
}