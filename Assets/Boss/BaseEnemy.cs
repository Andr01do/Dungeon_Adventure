using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Базові Характеристики")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float aggroRadius = 15f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float attackCooldown = 1.5f;
    public float xpReward = 50f;

    [Header("Анімації (Назви)")]
    public string animIdle = "Idle";
    public string animWalk = "Walk";
    public string animDeath = "Death";
    public string animDamage = "Damage";
    public string animAttack = "Attack";

    [Header("Аудіо")]
    public string sfxFootstep = "Step";
    public string sfxAttack = "Attack";
    public string sfxDamage = "Damage";
    public string sfxDeath = "Death";

    protected Transform target;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected bool isDead = false;
    protected bool isAttacking = false;
    protected float lastAttackTime = 0f;

    protected IAnimationService _animService;
    protected IAudioService _audioService;
    protected string currentAnimState = "";

    private float searchTimer = 0f;

    private float lastFootstepTime = 0f;
    private float lastAttackSoundTime = 0f;

    protected virtual void Start()
    {
        if (RunManager.Instance != null)
        {
            float multiplier = RunManager.Instance.GetDifficultyHealthMultiplier();
            maxHealth *= multiplier;
        }
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        try { _animService = ServiceLocator.Get<IAnimationService>(); } catch { }
        try { _audioService = ServiceLocator.Get<IAudioService>(); } catch { }

        FindPlayer();
    }

    protected virtual void Update()
    {
        if (isDead) return;
        if (target == null)
        {
            SearchForPlayerLogic();
            PlayAnimationSafe(animIdle);
            return;
        }

        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= aggroRadius)
        {
            if (distance > attackRange)
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    HandleMovement();
                }
            }
            else
            {
                if (agent.isOnNavMesh) agent.isStopped = true;

                HandleMovement(); 

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    StartCoroutine(PerformAttack());
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            PlayAnimationSafe(animIdle);
        }
    }

    protected virtual void HandleMovement()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            PlayAnimationSafe(animWalk);
        }
        else
        {
            PlayAnimationSafe(animIdle);
        }
    }

    protected abstract IEnumerator PerformAttack();

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (_audioService != null) _audioService.PlaySFX(sfxDamage, transform.position);

        if (!isAttacking)
        {
            PlayAnimationSafe(animDamage, 0.1f);
            Invoke(nameof(ResetAnimState), 0.5f);
        }

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        if (GameManager.Instance != null) GameManager.Instance.RegisterEnemyKill();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = false;

        GiveXPReward();

        PlayAnimationSafe(animDeath, 0.1f);
        if (_audioService != null) _audioService.PlaySFX(sfxDeath, transform.position);

        Destroy(gameObject, 4f);
    }

    protected void GiveXPReward()
    {
        if (target == null) return;
        float finalXP = xpReward;
        if (RunManager.Instance != null) finalXP *= RunManager.Instance.GetDifficultyXPMultiplier();
        var player = target.GetComponent<PlayerController>();
        if (player != null) player.GainExperience(finalXP);
    }

    protected void PlayAnimationSafe(string animName, float transition = 0.1f)
    {
        if (currentAnimState == animName) return;
        currentAnimState = animName;

        if (_animService != null && animator != null)
            _animService.PlayAnimation(animator, animName, transition);
        else if (animator != null)
            animator.CrossFade(animName, transition);
    }

    protected void ResetAnimState() => currentAnimState = "";

    private void SearchForPlayerLogic()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0)
        {
            FindPlayer();
            searchTimer = 1f;
        }
    }

    protected void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }


    public void OnFootstep()
    {
        if (isDead || _audioService == null) return;
        if (Time.time < lastFootstepTime + 0.3f) return;
        if (target != null && Vector3.Distance(transform.position, target.position) > 30f) return;

        _audioService.PlaySFX(sfxFootstep, transform.position);
        lastFootstepTime = Time.time;
    }

    public void OnAttackSound()
    {
        if (isDead || _audioService == null) return;
        if (Time.time < lastAttackSoundTime + 0.5f) return;
        if (target != null && Vector3.Distance(transform.position, target.position) > 30f) return;

        _audioService.PlaySFX(sfxAttack, transform.position);
        lastAttackSoundTime = Time.time;
    }
}