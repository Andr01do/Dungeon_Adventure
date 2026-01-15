using UnityEngine;
using System;

public class AudioManager : MonoBehaviour, IAudioService
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            if (s.loop)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
    }

    public void PlaySFX(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            return;
        }

        GameObject soundObj = new GameObject("TempSFX_" + name);
        soundObj.transform.position = position;
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        AudioClip clipToPlay = s.clip;

        if (s.variations != null && s.variations.Length > 0)
        {
            clipToPlay = s.variations[UnityEngine.Random.Range(0, s.variations.Length)];
        }

        if (clipToPlay == null)
        {
            Destroy(soundObj);
            return;
        }

        audioSource.clip = clipToPlay;

        float randomVol = UnityEngine.Random.Range(1f - s.volumeVariance, 1f + s.volumeVariance);
        float randomPitch = UnityEngine.Random.Range(1f - s.pitchVariance, 1f + s.pitchVariance);

        audioSource.volume = s.volume * randomVol;
        audioSource.pitch = s.pitch * randomPitch;
        audioSource.spatialBlend = s.spatialBlend;
        audioSource.minDistance = s.minDistance;
        audioSource.maxDistance = s.maxDistance;

        audioSource.Play();
        float safePitch = (Mathf.Abs(audioSource.pitch) < 0.01f) ? 1f : audioSource.pitch;

        Destroy(soundObj, clipToPlay.length / safePitch + 0.1f);
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.source == null) return;
        if (!s.source.isPlaying) s.source.Play();
    }

    public void StopMusic(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.source == null) return;
        s.source.Stop();
    }
}