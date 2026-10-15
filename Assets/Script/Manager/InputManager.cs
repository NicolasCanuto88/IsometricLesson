using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // FONDAMENTALE!

[DefaultExecutionOrder(-100)] // Lo facciamo partire prima di tutti gli altri script
public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton
    public PlayerControls controls; // Riferimento al PlayerInput
    public InputAction Zoom => controls.Camera.Zoom; // Proprietà per accedere all'azione di zoom

    void Awake()
    {
        // Pattern Singleton standard
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        controls = new PlayerControls(); // Inizializziamo i controlli
    }

    private void OnEnable()
    {
        controls.Enable(); 

        controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);

        controls.Player.Run.performed += ctx => OnRun?.Invoke(true);
        controls.Player.Run.canceled += ctx => OnRun?.Invoke(false);

        controls.Player.Attack.performed += ctx => OnAttack?.Invoke();

        controls.Player.Dash.performed += ctx => OnDash?.Invoke();

        controls.Player.Block.performed += ctx => OnBlock?.Invoke(true);
        controls.Player.Block.canceled += ctx => OnBlock?.Invoke(false);

        controls.Player.LockOn.performed += ctx => OnLockOn?.Invoke();

    }
    private void OnDisable() { if(controls != null) { controls.Disable(); } }

    public event System.Action<Vector2> OnMove; // Evento per il movimento (X,Y)
    public event System.Action<bool> OnRun; // Evento per la corsa
    public event System.Action OnAttack; // Evento per l'attacco

    public event System.Action OnDash; // Evento per il dash
    public event System.Action<bool> OnBlock; // Evento per il blocco
    public event System.Action OnLockOn; // Evento per il lock-on

}
