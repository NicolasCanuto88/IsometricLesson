using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IPlayerState
{
    private PlayerController player;
    public WalkingState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        Debug.Log("Walking State Entered");
    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {
        Debug.Log("Walking State Exited");
        player.HandleMovement();
    }

    public void Update()
    {
        // 1. Se non c'è più input -> Torna in Idle State
        if (!player.HasMovementInput())
        {
            // Se non c'è input di movimento, torniamo allo stato Idle
            player.TransitionToState(player.idleState);
            return;
        }

        // 2. Se c'è input di corsa -> Vai in Running State
        if(player.IsRunningInput())
        {
            player.TransitionToState(player.runningState);
            return;
        }

        // 3. LETTURA DEL TARGET LOCK
        TargetLock targetLock = player.GetComponent<TargetLock>();
        bool isLocked = targetLock != null && targetLock.isLockedOn;

        // Passiamo l'informazione al compenete Animator
        player.PlayerAnim.SetLockedOn(isLocked);

        if(isLocked)
        {
            Transform cam = Camera.main.transform;
            Vector3 camForward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
            Vector3 camRight = new Vector3(cam.right.x, 0, cam.right.z).normalized;

            Vector3 wordlMoveDir = (camForward * player.currentMoveInput.y + camRight * player.currentMoveInput.x).normalized;

            Vector3 localMoveDir = player.transform.InverseTransformDirection(wordlMoveDir);

            player.PlayerAnim.UpdateBlendTree2D(localMoveDir.x * 0.5f, localMoveDir.z * 0.5f);
        }
        else
        {
            player.PlayerAnim.UpdateBlendTree(0.5f); // Se non siamo lockati, usiamo un blend tree semplice che si basa solo sulla magnitudine dell'input   
        }

    }
}
