using UnityEngine;

public interface IAudioService
{
    void PlaySFX(string name, Vector3 position);
    void PlayMusic(string name);
    void StopMusic(string name);
}