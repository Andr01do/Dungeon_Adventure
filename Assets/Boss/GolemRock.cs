using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GolemRock : MonoBehaviour
{
    private float damage;
    private bool hasHit = false;
    public void Initialize(float dmg)
    {
        damage = dmg;
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        if (collision.gameObject.CompareTag("Enemy")) return;

        hasHit = true;
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}