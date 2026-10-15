using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : IPlayerState
{
    private PlayerController player;
    public RunningState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
    }

    public void Exit()
    {
        player.PlayerAnim.SetAnimatorSpeed(1f); // PER SICUREZZA, RESETTIAMO LA VELOCITÀ DELL'ANIMATOR A 1 QUANDO USCIREMO DALLO STATO DI CORSA
    }

    public void FixedUpdate()
    {
        player.HandleMovement();
    }

    public void Update()
    {
        // Se non c'è Input -> Torna in Idle State
        if (!player.HasMovementInput())
        {
            player.TransitionToState(player.idleState);
            return;
        }

        // Se non c'è più input di corsa -> Vai in Walking State
        if (!player.IsRunningInput())
        {
            player.TransitionToState(player.walkingState);
            return;
        }

        TargetLock targetLock = player.GetComponent<TargetLock>();
        bool isLocked = targetLock != null && targetLock.isLockedOn;

        player.PlayerAnim.SetLockedOn(isLocked);

        if (isLocked)
        {
            Transform cam = Camera.main.transform;
            Vector3 camForward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
            Vector3 camRight = new Vector3(cam.right.x, 0, cam.right.z).normalized;
            Vector3 wordlMoveDir = (camForward * player.currentMoveInput.y + camRight * player.currentMoveInput.x).normalized;
            Vector3 localMoveDir = player.transform.InverseTransformDirection(wordlMoveDir);

            if(localMoveDir.z > 0.1f)
            {
                player.PlayerAnim.SetAnimatorSpeed(1f);
            }
            else
            {
                player.PlayerAnim.SetAnimatorSpeed(1.5f);
            }
            
            player.PlayerAnim.UpdateBlendTree2D(localMoveDir.x, localMoveDir.z);
        }
        else
        {
            player.PlayerAnim.SetAnimatorSpeed(1f); // PER SICUREZZA, RESETTIAMO LA VELOCITÀ DELL'ANIMATOR A 1 QUANDO NON SIAMO LOCKATI
            player.PlayerAnim.UpdateBlendTree(1f);
        }
    }
}
