using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
    public int damage = 20;
    public float damageInterval = 1f;

    private float nextDamageTime;

    void OnTriggerStay(Collider other)
    {
        if (Time.time < nextDamageTime) return;

        PlayerHealth health =
            other.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }
}
