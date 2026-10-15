using UnityEngine;

public interface IDamageable
{
    DamageResult TakeDamage(int damageAmount, GameObject attacker = null);
}
