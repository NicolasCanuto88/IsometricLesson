using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadState : IPlayerState
{
    private PlayerController player;

    public DeadState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.PlayerAnim.SetUpdateModeToUnscaled(); // Imposta l'animazione per ignorare la scala del tempo

        //1. Attivare animazione di morte
        player.PlayerAnim.TriggerDeathAnimation();

        //2. Disabilitare il movimento del giocatore
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero; // Ferma il movimento
            rb.isKinematic = true; // Rende il Rigidbody kinematic per evitare interazioni fisiche
        }

        // 3. Fermiamo il gioco (il tempo)
        Time.timeScale = 0f; // Ferma il tempo, congelando tutto
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            // Riavvia la scena o esegui altre azioni per resettare il gioco
            Time.timeScale = 1f; // Riporta il tempo alla normalità
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Ricarica la scena attuale
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }


}
