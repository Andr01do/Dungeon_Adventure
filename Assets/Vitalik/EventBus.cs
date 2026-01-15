using System;
using UnityEngine;

public class EventBus : IEventService
{
    public event Action OnPlayerDied;
    public event Action<float> OnPlayerTookDamage;


    public void TriggerPlayerDied()
    {
        Debug.Log("[EventBus] Player Died!");
        OnPlayerDied?.Invoke();
    }


    public void TriggerPlayerTookDamage(float damage)
    {
        OnPlayerTookDamage?.Invoke(damage);
    }

}