using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxTest : MonoBehaviour, IDamageable
{
    public int maxHealth = 30;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }


    public DamageResult TakeDamage(int damageAmount, GameObject attacker = null)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Nemico colpito! Danno ricevuto: {damageAmount}, Salute rimanente: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        return DamageResult.Hit;
    }

    private void Die()
    {
        Debug.Log("Nemico sconfitto!");
        // Qui puoi aggiungere logica per la morte del nemico, come animazioni, distruzione dell'oggetto, ecc.
        Destroy(gameObject);
    }


}
