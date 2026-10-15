using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingState : IPlayerState
{
    private PlayerController player;
    public BlockingState(PlayerController player) { this.player = player; }
    public void Enter()
    {
        var health = player.GetComponent<PlayerHealth>();
        health.isBlocking = true;
        health.StartParryWindow(); // Avvia la finestra di parry quando il giocatore entra nello stato di blocco
        player.PlayerAnim.SetBlocking(true);  
        
        Debug.Log("Blocking State Entered");
    }

    public void Update()
    {
        TargetLock targetLock = player.GetComponent<TargetLock>();
        bool isLocked = targetLock != null && targetLock.isLockedOn;

        player.PlayerAnim.SetLockedOn(isLocked);

        if (isLocked)
        {
            if (player.HasMovementInput())
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
                player.PlayerAnim.UpdateBlendTree2D(0f, 0f);
            }
        }
        else
        {
            if (player.HasMovementInput())
            {
                player.PlayerAnim.UpdateBlendTree(0.5f);

            }
            else
            {
                player.PlayerAnim.UpdateBlendTree(0f);
            }
        }
    }

    public void FixedUpdate()
    {
        player.HandleMovement(); // Permetti al giocatore di muoversi anche mentre blocca
    }

    public void Exit()
    {
        player.GetComponent<PlayerHealth>().isBlocking = false;
        player.PlayerAnim.SetBlocking(false);
    }

}
