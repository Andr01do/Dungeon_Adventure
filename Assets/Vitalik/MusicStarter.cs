using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    public string musicTrackName = "Background";

    void Start()
    {
        var audioService = ServiceLocator.Get<IAudioService>();
        audioService?.PlayMusic(musicTrackName);
    }
}
