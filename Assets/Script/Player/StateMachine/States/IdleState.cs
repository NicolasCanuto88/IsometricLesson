using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerController player;
    public IdleState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        Debug.Log("Idle State Entered");
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // Ferma il movimento del Rigidbody
        }
    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {
        Debug.Log("Idle State Exited");

    }

    public void Update()
    {
        //1. VERIFICHIAMO SE IL PLAYER AGGANCIA IL NEMICO
        TargetLock targetLock = player.GetComponent<TargetLock>();
        bool isLocked = targetLock != null && targetLock.isLockedOn;

        player.PlayerAnim.SetLockedOn(isLocked); 

        if(isLocked)
        {
            player.PlayerAnim.UpdateBlendTree2D(0f, 0f);
        }
        else
        {
            player.PlayerAnim.UpdateBlendTree(0f);
        }

        // Se riceviamo input di movimento, cambiamo stato
        if (player.HasMovementInput())
        {
            // Decidiamo se andare in Walking o Running state
            if(player.IsRunningInput())
                player.TransitionToState(player.runningState);
            else
                player.TransitionToState(player.walkingState);
        }
    }
}
