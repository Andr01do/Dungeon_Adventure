using UnityEngine;

public class PlayerMovingState : PlayerBaseState
{
    private IAnimationService animService;

    private const string ANIMATION_NAME = "Walking";

    public PlayerMovingState(PlayerController controller) : base(controller)
    {
        try
        {
            animService = ServiceLocator.Get<IAnimationService>();
        }
        catch { animService = null; }
    }

    public override void Enter()
    {
        controller.IsRegenPaused = false;
        if (animService != null)
        {
            animService.PlayAnimation(controller.animator, ANIMATION_NAME, 0.1f);
        }
    }

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            controller.Jump();
            return;
        }

        if (Input.GetMouseButtonDown(0) && controller.HasStamina(controller.attackStaminaCost))
        {
            controller.ChangeState(controller.combatState);
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && controller.HasStamina(5f))
        {
            controller.ChangeState(controller.sprintingState);
            return;
        }

        if (controller.moveDirection.magnitude < 0.1f)
        {
            controller.ChangeState(controller.idleState);
            return;
        }
    }

    public override void Update()
    {
        controller.RegenerateStamina();
    }

    public override void FixedUpdate()
    {
        controller.ApplyMovement(controller.moveSpeed);
        controller.HandleRotation();
    }
}