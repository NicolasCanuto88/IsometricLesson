using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 30;
    private int currentHealth;

    [Header("Effect")]
    [SerializeField] private GameObject hitSparkPrefab;
    [SerializeField] private GameObject blockSparkPrefab;

    [Header("Combat Status")]
    public bool isDefending = false; // Stato di difesa del nemico

    private void Start()
    {
        currentHealth = maxHealth;
    }


    public DamageResult TakeDamage(int damageAmount, GameObject attacker = null)
    {
        if (isDefending)
        {
            Debug.Log("Attacco parato dal cavaliere! Nessun danno subito");

            if (blockSparkPrefab != null)
            {
                Instantiate(blockSparkPrefab, transform.position + Vector3.up * 1.3f + transform.forward * 0.5f, Quaternion.identity);
            }

            return DamageResult.Blocked; // Esci dalla funzione senza applicare danni
        }

        currentHealth -= damageAmount;

        if(hitSparkPrefab != null)
        {
            Instantiate(hitSparkPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

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
