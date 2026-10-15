using UnityEngine;

public class StateMachineController
{
    // Tiene traccia dello stato corrente del giocatore
    public IPlayerState currentState;

    public void Initialize(IPlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(IPlayerState newState)
    {
        if(currentState != null)
        {
            // Usciamo dal vecchio stato
            currentState.Exit();
        }
        

        // Entriamo nel nuovo stato
        currentState = newState;
        currentState.Enter();
    }
    public void Update()
    {
        // Eseguiamo l'aggiornamento della logica dello stato corrente
        if(currentState != null)
        {
            currentState.Update();
        }
    }
    public void FixedUpdate()
    {
        // Eseguiamo l'aggiornamento fisico dello stato corrente
        if(currentState != null)
        {
            currentState.FixedUpdate();
        }
    }
}
