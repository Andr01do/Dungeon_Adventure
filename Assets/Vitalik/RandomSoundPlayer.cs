using UnityEngine;
using System.Collections;

public class RandomSoundPlayer : MonoBehaviour
{
    public string[] soundNames;
    public float minDelay = 5f; 
    public float maxDelay = 15f;

    void Start()
    {
        if (ServiceLocator.Get<IAudioService>() != null)
            StartCoroutine(PlayRandomSoundsRoutine());
    }

    IEnumerator PlayRandomSoundsRoutine()
    {
        var audioService = ServiceLocator.Get<IAudioService>();
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            if (soundNames.Length > 0)
            {
                string randomName = soundNames[Random.Range(0, soundNames.Length)];
                audioService.PlaySFX(randomName, transform.position);
            }
        }
    }
}
