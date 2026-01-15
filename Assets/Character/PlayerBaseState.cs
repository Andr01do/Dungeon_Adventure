using UnityEngine;

public abstract class PlayerBaseState : IPlayerState 
{
    protected PlayerController controller;
    protected IAnimationService animService; 

    protected PlayerBaseState(PlayerController controller)
    {
        this.controller = controller;
        this.animService = ServiceLocator.Get<IAnimationService>();
    }

    public abstract void Enter();
    public abstract void HandleInput();
    public abstract void Update();
    public abstract void FixedUpdate();
    public virtual void Exit() { }
}