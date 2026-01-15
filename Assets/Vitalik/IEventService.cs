using System;
using UnityEngine;

public interface IEventService
{

    event Action OnPlayerDied;

    event Action<float> OnPlayerTookDamage; 


    void TriggerPlayerDied();
    void TriggerPlayerTookDamage(float damage);
}