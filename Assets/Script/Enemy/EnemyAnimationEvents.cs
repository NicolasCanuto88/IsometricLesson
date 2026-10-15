using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private WeaponHitbox weaponHitbox; // Riferimento al hitbox dell'arma

    private EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    public void OpenWeaponHitbox()
    {
        if (weaponHitbox != null) weaponHitbox.EnableHitbox();
    }

    public void CloseWeaponHitbox()
    {
        if (weaponHitbox != null) weaponHitbox.DisableHitbox();
    }

    public void TriggerFinishAttack()
    {
        if (enemyAI != null) enemyAI.FinishAttack();
    }
}
