using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GameEntity
{
    [Header("Dependencies")]
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private PlayerAnimator playerAnimator;
    public PlayerAnimator PlayerAnim => playerAnimator;

    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkingState walkingState;
    [HideInInspector] public RunningState runningState;
    [HideInInspector] public AttackingState attackingState;
    [HideInInspector] public DashingState dashingState;
    [HideInInspector] public BlockingState blockingState;
    [HideInInspector] public DeadState deadState;

    private StateMachineController stateMachine;

    [HideInInspector] public Vector2 currentMoveInput; // Per tenere traccia dell'input di movimento corrente

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField] private float stateDuration = 0.7f;

    public float DashSpeed => dashSpeed;
    public float SlideDuration => slideDuration;
    public float StateDuration => stateDuration;

    protected override void Start()
    {
        // Chiamiamo il metodo Start della classe base
        base.Start();

        // Se non assegnato, cerchiamo il componente MovementComponent nello stesso GameObject
        if (movementComponent == null) movementComponent = GetComponent<MovementComponent>();
        if (playerAnimator == null) playerAnimator = GetComponent<PlayerAnimator>();


        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);
        attackingState = new AttackingState(this);
        dashingState = new DashingState(this);
        blockingState = new BlockingState(this);
        deadState = new DeadState(this);

        stateMachine = new StateMachineController();
        stateMachine.Initialize(idleState);

        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack += HandleAttackInput;
            InputManager.Instance.OnMove += HandleMoveInput;
            InputManager.Instance.OnDash += HandleDashInput;
            InputManager.Instance.OnBlock += HandleBlockInput;

        }


    }
    private void Update() { stateMachine.Update(); }

    private void OnDestroy()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack -= HandleAttackInput;
            InputManager.Instance.OnMove -= HandleMoveInput;
            InputManager.Instance.OnDash -= HandleDashInput;
            InputManager.Instance.OnBlock -= HandleBlockInput;
        }
    }


    private void FixedUpdate() { stateMachine.FixedUpdate();}

    public void TransitionToState(IPlayerState newState){ stateMachine.ChangeState(newState); }

    public bool HasMovementInput() => movementComponent.HasMovementInput();
    public bool IsRunningInput() => movementComponent.IsRunningInput();
    public void HandleMovement() => movementComponent.HandleIsometricMovement();

    private void HandleMoveInput(Vector2 input) => currentMoveInput = input;

    public void HandleAttack() => HandleAttackInput();

    public void HandleAttackInput()
    {
        if (stateMachine != null && stateMachine.currentState != attackingState && stateMachine.currentState != deadState)
        {
            TransitionToState(attackingState);
        }
    }

    private void HandleDashInput()
    {
        if(stateMachine != null && stateMachine.currentState != dashingState && stateMachine.currentState != deadState)
        {
            TransitionToState(dashingState);
        }
    }

    private void HandleBlockInput(bool isHoldingBlock)
    {
        if (stateMachine == null && stateMachine.currentState == deadState) return;

        if(isHoldingBlock && (stateMachine.currentState == idleState || stateMachine.currentState == walkingState || stateMachine.currentState == runningState)) 
            TransitionToState(blockingState);
        else if (!isHoldingBlock && stateMachine.currentState == blockingState)
            {
                TransitionToState(idleState);
            }
        
    }

    public void OnAttackAnimationComplete()
    {
        TransitionToState(idleState);
    }

    
}

