using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingState : IPlayerState
{
    private PlayerController player;

    private float dashTimer;
    private Vector3 dashDirection;

    public bool isBackstepping;

    public DashingState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        dashTimer = 0;
        player.GetComponent<PlayerHealth>().isInvincible = true; // Rendi il giocatore invincibile durante il dash
        player.PlayerAnim.TriggerDashAnimation();

        Vector2 input = player.currentMoveInput;

        if (input.magnitude > 0.1f)
        {
            isBackstepping = false;
            Transform cam = Camera.main.transform;

            Vector3 camForward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
            Vector3 camRight = new Vector3(cam.right.x, 0, cam.right.z).normalized;

            dashDirection = (camForward * input.y + camRight * input.x).normalized; // Calcola la direzione del dash in base all'input e alla direzione della telecamera

            if(dashDirection.sqrMagnitude > 0.1f)
                player.transform.rotation = Quaternion.LookRotation(dashDirection); // Ruota il giocatore verso la direzione del dash
        }
        else
        {
            isBackstepping = true;

            dashDirection = -player.transform.forward; // Se non c'è input, dash nella direzione in cui il giocatore sta guardando
            if(dashDirection.sqrMagnitude > 0.1f)
                player.transform.rotation = Quaternion.LookRotation(dashDirection);
        }
    }

    public void Update()
    {
        dashTimer += Time.deltaTime;

        // Aspettiamo che TUTTA l'animazione finisca
        if (dashTimer >= player.StateDuration) 
            player.TransitionToState(player.idleState); // Torna allo stato Idle dopo la durata del dash
    }

    public void FixedUpdate()
    {
        // Applica la forza solo durante la fase di scivolamento
        if (dashTimer < player.SlideDuration)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = new Vector3(dashDirection.x * player.DashSpeed, rb.velocity.y, dashDirection.z * player.DashSpeed); // Applica una forza costante nella direzione del dash
            }
        } 
            
    }

    public void Exit()
    {
        player.GetComponent<PlayerHealth>().isInvincible = false; // Rendi il giocatore nuovamente vulnerabile dopo il dash
        if(isBackstepping)
            player.transform.rotation = Quaternion.LookRotation(-dashDirection); // Ruota il giocatore indietro se era un backstep
    }
}
