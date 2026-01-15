using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip[] variations;
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 0.7f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    [Tooltip("Наскільки сильно може змінюватися гучність (+/-)")]
    [Range(0f, 0.5f)] public float volumeVariance = 0.1f;

    [Tooltip("Наскільки сильно може змінюватися пітч (+/-)")]
    [Range(0f, 0.5f)] public float pitchVariance = 0.1f;

    [Range(0f, 1f)] public float spatialBlend = 1f; 
    public float minDistance = 2f;
    public float maxDistance = 50f;

    public bool loop;

    [HideInInspector] public AudioSource source;
}