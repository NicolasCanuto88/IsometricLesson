using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    // Animazione Del Nemico -> OnTriggerEnter -> IDamagable.TakeDamage() -> PlayerHealth

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [HideInInspector] public bool isInvincible = false; // Variabile per gestire l'invincibilità temporanea
    [HideInInspector] public bool isBlocking = false; // Variabile per gestire se il player sta bloccando

    private PlayerController playerController;

    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.2f; // Finestra temporale per il parry
    private float parryTimer = 0f;

    public bool IsParrying => parryTimer > 0f; // Proprietà per verificare se il player sta parando

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(parryTimer > 0) parryTimer -= Time.deltaTime;
    }

    public DamageResult TakeDamage(int damageAmount, GameObject attacker = null)
    {
        if (currentHealth <= 0 || isInvincible) return DamageResult.Hit; // Se il player è già morto oppure sta bloccando, non subisce ulteriori danni

        //1: Parry
        if (IsParrying)
        {
            parryTimer = 0f; // Resetta il timer del parry
            Debug.Log("PARRY RIUSCITO!");
            return DamageResult.Parried;
        }

        //2: Block
        if (isBlocking)
        {
            Debug.Log("Il Player ha bloccato l'attacco e non subisce danno.");
            return DamageResult.Blocked;
        }

        //3: DANNO
        currentHealth -= damageAmount;
        Debug.Log("Il Player ha subito danno: " + damageAmount + ". La salute attuale è: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        return DamageResult.Hit;
    }

    private void Die()
    {
        // Qui puoi aggiungere logica per la morte del player, come animazioni, respawn, ecc.

        if(playerController != null)
        {
            playerController.TransitionToState(playerController.deadState);
        }
    }

    public void StartParryWindow() => parryTimer = parryWindow;
}
