using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Best Practice: Attributo che assicura che il RigidBody sia sempre presente sull'oggetto
[RequireComponent(typeof(Rigidbody))]
public class MovementComponent : MonoBehaviour
{
    [Header("Settings")]
    // Usiamo SerializeField per esporre le variabili private nell'Inspector di Unity
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    // Variabili interne (non visibili nell'Inspector)
    private Rigidbody rb;
    private Vector2 inputDirection;
    private bool isRunning;

    private TargetLock targetLock;

    private void Awake()
    {
        // Otteniamo il riferimento al RigidBody
        rb = GetComponent<Rigidbody>();

        targetLock = GetComponent<TargetLock>();    
    }

    private void OnEnable()
    {
        if(InputManager.Instance != null)
        {
            // Registriamo i callback per gli eventi di input
            InputManager.Instance.OnMove += HandleMoveInput;
            InputManager.Instance.OnRun += HandleRunToggle;
        }
    }

    private void OnDisable()
    {
        if(InputManager.Instance != null)
        {
            // Deregistriamo i callback per evitare memory leak
            InputManager.Instance.OnMove -= HandleMoveInput;
            InputManager.Instance.OnRun -= HandleRunToggle;
        }
    }


    // Callback per gli eventi
    private void HandleMoveInput(Vector2 input) => inputDirection = input;
    private void HandleRunToggle(bool runState) => isRunning = runState;

    // Metodi pubblici di utility (getters)
    public bool HasMovementInput() => inputDirection != Vector2.zero;
    public bool IsRunningInput() => isRunning;
    

    // LOGICA DI MOVIMENTO ISOMETRICO
    public void HandleIsometricMovement()
    {
        // Se non c'è input, fermiamo il movimento orizzontale
        if (inputDirection == Vector2.zero)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Ferma il movimento orizzontale
            return;
        }

        
        // 1. Calcolo direzione di movimento isometrica (relativa alla Camera)
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Appiattiamo la Y per non volare/sprofondare
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Combinazione dei vettori
        Vector3 moveDir = (camForward * inputDirection.y + camRight * inputDirection.x).normalized;

        // 2. Calcolo della velocità Analogica
        // Decidiiamo il limite massimo consentito (es. 10 se corre, 5 se cammina)
        float maxSpeed = isRunning ? runSpeed : walkSpeed;

        float actualSpeed = maxSpeed * Mathf.Clamp01(inputDirection.magnitude); // Velocità proporzionale all'inclinazione della levetta

        // 3. Applicazione velocità al RigidBody
        Vector3 velocity = moveDir * actualSpeed;

        // Assegniazione della velocità del rigidBody mantenendo la componente Y (gravità, salti, ecc.)
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // 4. Rotazione del personaggio verso la direzione di movimento

        if(targetLock != null && targetLock.isLockedOn && targetLock.currentTarget != null)
        {
            Vector3 directionToEnemy = targetLock.currentTarget.position - transform.position;
            directionToEnemy.y = 0; // Ignoriamo la componente verticale
            if(directionToEnemy != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f); // Rotazione fluida verso il nemico
            }
        }

        else if(moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }
}


