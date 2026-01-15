using UnityEngine;
using System.Collections;
using System;

public class BossGolem : BaseEnemy
{
    [Header("Специфіка Боса")]
    public string animRun = "Run";
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeathEvent;

    [Header("Дальня Атака (Камінь)")]
    public GameObject rockPrefab;     
    public Transform handTransform;    
    public string animRangeAttack = "Attack2_Golem"; 
    public float throwRangeMin = 5f;    
    public float throwRangeMax = 15f; 
    public float throwCooldown = 5f;   
    public float rockDamage = 20f;
    public float rockFlightTime = 1.5f; 

    private float lastThrowTime = -10f; 

    protected override void Start()
    {
        base.Start();
        agent.speed = 2.5f;
        agent.acceleration = 4f;
    }

    protected override void Update()
    {
        if (isDead) return;

        if (isAttacking)
        {
            if (target != null) RotateTowards(target.position);
            return;
        }

        HandleMovement();

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > aggroRadius) return;

        bool canThrow = Time.time >= lastThrowTime + throwCooldown;

        if (distance > throwRangeMin && distance <= throwRangeMax && canThrow)
        {
            StartCoroutine(PerformRangeAttack());
        }
        else if (distance <= throwRangeMin)
        {
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance + attackRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    StartCoroutine(PerformAttack());
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }
    protected override IEnumerator PerformAttack()
    {
        isAttacking = true;
        agent.isStopped = true;

        PlayAnimationSafe(animAttack, 0.1f);

        yield return new WaitForSeconds(0.8f);

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= attackRange + 1.5f)
            {
                IDamageable hit = target.GetComponent<IDamageable>();
                if (hit != null) hit.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
        if (!isDead && agent != null && agent.isOnNavMesh) agent.isStopped = false;
        ResetAnimState();
    }

    private IEnumerator PerformRangeAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        lastThrowTime = Time.time;

        PlayAnimationSafe(animRangeAttack, 0.1f);
        if (target != null) RotateTowards(target.position);

        yield return new WaitForSeconds(0.8f);

        if (target != null)
        {
            ThrowRockAtTarget();
        }
        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
        if (!isDead && agent != null && agent.isOnNavMesh) agent.isStopped = false;
        ResetAnimState();
    }

    private void ThrowRockAtTarget()
    {
        if (rockPrefab == null || handTransform == null || target == null) return;
        GameObject rockObj = Instantiate(rockPrefab, handTransform.position, Quaternion.identity);
        GolemRock rockScript = rockObj.GetComponent<GolemRock>();
        if (rockScript) rockScript.Initialize(rockDamage);
        Rigidbody rb = rockObj.GetComponent<Rigidbody>();

        Vector3 targetPos = target.position + Vector3.up * 1.0f;
        Vector3 velocity = CalculateLaunchVelocity(handTransform.position, targetPos, rockFlightTime);

        rb.linearVelocity = velocity; 

        rb.angularVelocity = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), 0);
    }

    private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 end, float time)
    {
        Vector3 distance = end - start;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float sY = distance.y;
        float sXZ = distanceXZ.magnitude;

        float Vxz = sXZ / time;
        float Vy = sY / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    private void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    protected override void HandleMovement()
    {
        float speed = agent.velocity.magnitude;
        string newAnim = (speed > 4f) ? animRun : ((speed > 0.1f) ? animWalk : animIdle);
        PlayAnimationSafe(newAnim, 0.2f);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected override void Die()
    {
        if (isDead) return;

        Debug.Log("BOSS DEFEATED!");
        OnDeathEvent?.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBossDefeated();
        }

        base.Die();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius); 

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, throwRangeMax);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, throwRangeMin);
    }
}