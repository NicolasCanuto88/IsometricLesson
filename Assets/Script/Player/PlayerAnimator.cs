using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;

    private Vector2 currentInput;
    private bool isRunning; // Sapere se il tasto Run è premuto

    private float currenstSpeedValue; // Valore attuale della velocità verticale (per blend tree)
    public float acceleration = 5f; // Quanto velocemente cambia la velocità (per blend tree)

    [Header("Dependencies")]
    [SerializeField] private WeaponHitbox weaponHitbox; // Riferimento al hitbox dell'arma

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnMove += UpdateMovementInput;
            InputManager.Instance.OnRun += UpdateRunInput; // Aggiorniamo lo stato di corsa
        }
    }

    void UpdateMovementInput(Vector2 input) { currentInput = input; }
    void UpdateRunInput(bool run) { isRunning = run; }

    // Update is called once per frame
    void Update()
    {
        float targetSpeed = 0f;
        if(currentInput.magnitude > 0.1f)
        {
            // Decidiamo il limite di velocità in base a se stiamo correndo o camminando (1.0 per corsa, 0.5 per camminata)
            float speedLimit = isRunning ? 1f : 0.5f;

            //Moltiplichiamo l'inclinazione della levetta per il limite di velocità.
            // Se spingo la levetta a metà (0.5) e sto camminando, voglio che il valore finale sia 0.25 (0.5 * 0.5)
            targetSpeed = currentInput.magnitude * speedLimit;

            // Sicurezza: blocchiamo il valore per non fargli superare il limite
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, speedLimit);
        }
        currenstSpeedValue = Mathf.MoveTowards(currenstSpeedValue, targetSpeed, acceleration * Time.deltaTime);

        anim.SetFloat("Speed", currenstSpeedValue);
    }

    public void TriggerAttackAnimation()
    {
        anim.SetTrigger("Attack");
    }

    public void TriggerDeathAnimation()
    {
        anim.SetTrigger("Dead");
    }

    public void SetUpdateModeToUnscaled()
    {
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void OpenWeaponHitbox()
    {
        if(weaponHitbox != null) weaponHitbox.EnableHitbox();
    }

    public void CloseWeaponHitbox()
    {
        if(weaponHitbox != null) weaponHitbox.DisableHitbox();
    }

    public void TriggerDashAnimation()
    {
        anim.SetTrigger("Dash");
    }

    public void SetBlocking(bool isBlocking)
    {
        anim.SetBool("IsBlocking", isBlocking);
    }

    public void SetLockedOn(bool isLocked)
    {
        anim.SetBool("IsLockedOn", isLocked);
    }

    public void UpdateBlendTree2D(float x, float y)
    {
        anim.SetFloat("InputX", x);
        anim.SetFloat("InputY", y);
    }

    public void UpdateBlendTree(float speedValue)
    {
        anim.SetFloat("Speed", speedValue);
    }

    public void SetAnimatorSpeed(float speedMultiplier)
    {
        anim.speed = speedMultiplier;
    }

    private void OnDestroy()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnMove -= UpdateMovementInput; // Puliamo l'evento per evitare memory leak
            InputManager.Instance.OnRun -= UpdateRunInput; // Puliamo anche questo
        }
    }
}



