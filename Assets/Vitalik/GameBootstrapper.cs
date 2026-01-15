using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public AudioManager audioManagerPrefab;
    void Awake()
    {
        var audioManager = FindAnyObjectByType<AudioManager>();
        ServiceLocator.Register<IAudioService>(audioManager);
        ServiceLocator.Register<IAnimationService>(new AnimationManager());
        ServiceLocator.Register<IEventService>(new EventBus());
    }
}
