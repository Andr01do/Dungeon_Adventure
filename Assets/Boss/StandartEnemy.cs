using UnityEngine;
using System.Collections;

public class StandardEnemy : BaseEnemy
{
    [Header("Специфіка Мобів")]
    [Tooltip("Список анімацій для випадкової атаки")]
    public string[] attackAnimations;

    [Header("Лут")]
    [Range(0f, 1f)] public float potionDropChance = 0.25f;
    public GameObject potionPrefab;
    public float dropHeightOffset = 0.5f;

    protected override IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Гарантована зупинка
        if (agent != null && agent.enabled) agent.isStopped = true;

        // Вибір анімації
        string chosenAnim = animAttack;
        if (attackAnimations != null && attackAnimations.Length > 0)
        {
            chosenAnim = attackAnimations[Random.Range(0, attackAnimations.Length)];
        }

        PlayAnimationSafe(chosenAnim, 0.1f);

        // --- ВИПРАВЛЕННЯ ---
        // Я закоментував цей рядок. Нехай звук грає через Animation Event "OnAttackSound".
        // Якщо подій в анімації немає - розкоментуй це.
        // if (_audioService != null) _audioService.PlaySFX(sfxAttack, transform.position);

        // Чекаємо половину удару (замах)
        yield return new WaitForSeconds(0.4f); // Трохи менше ніж 0.5, щоб удар був синхронним

        // Перевірка, чи ми не померли поки чекали
        if (isDead) yield break;

        // Нанесення шкоди
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange + 1.5f) // +1.5f запас
        {
            IDamageable damageableTarget = target.GetComponent<IDamageable>();
            if (damageableTarget != null) damageableTarget.TakeDamage(damage);
        }

        // Чекаємо завершення анімації
        yield return new WaitForSeconds(0.8f);

        if (isDead) yield break;

        // Повертаємо рухливість
        if (agent != null && agent.enabled) agent.isStopped = false;

        isAttacking = false;

        // Скидаємо стейт анімації, щоб Update міг перемкнути на Idle/Walk
        ResetAnimState();
    }

    protected override void Die()
    {
        // Викликаємо дроп ПЕРЕД базовою смертю, бо base.Die() може видалити компоненти
        TryDropPotion();
        base.Die();
    }

    private void TryDropPotion()
    {
        if (Random.value <= potionDropChance && potionPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * dropHeightOffset;
            Instantiate(potionPrefab, spawnPos, Quaternion.identity);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}