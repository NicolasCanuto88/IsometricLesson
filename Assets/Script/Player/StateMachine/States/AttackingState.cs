using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackingState : IPlayerState
{
    private PlayerController player;

    private float safetyTimer;
    private float maxAttackDuration = 1.5f; // Durata massima dell'attacco per sicurezza

    public AttackingState(PlayerController player)
    {
        this.player = player;
    }



    public void Enter()
    {
        safetyTimer = 0f; // Resettiamo il timer di sicurezza

        player.PlayerAnim.TriggerAttackAnimation();
        // 2. Ferma il movimento del giocatore
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        safetyTimer += Time.deltaTime;
        if(safetyTimer >= maxAttackDuration)
        {
            Debug.LogWarning("Attacco durato troppo a lungo, tornando allo stato Idle per sicurezza.");
            // Se l'attacco dura troppo a lungo, torniamo allo stato Idle per sicurezza
            player.TransitionToState(player.idleState);
        }
    }
    public void FixedUpdate()
    {
        // Non facciamo nulla qui, vogliamo che il giocatore resti fermo durante l'attacco
    }
    public void Exit()
    {
        // Non facciamo nulla qui, ma potremmo resettare variabili o trigger se necessario
    }
}
