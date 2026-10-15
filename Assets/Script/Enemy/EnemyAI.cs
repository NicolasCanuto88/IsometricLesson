using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IStaggerable
{
    [Header("Targeting")]
    public Transform player;
    public float chaseRange = 10f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    [Header("Vision Settings")]
    [SerializeField] float fieldOfViewAngle = 90f; // Angolo di visione del nemico
    [SerializeField] LayerMask obstacleLayer; // Layer degli ostacoli che bloccano la vista del nemico

    [Header("Combat Settings")]
    [SerializeField] float chaseSpeed = 4f;

    [Header("Tactical Combat Settings")]
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float timeBetweenAttacks = 4f;
    private float nextAttackTime;

    [SerializeField] float recoveryTime = 1.5f;
    private float endRecoveryTime;

    [HideInInspector] public bool isAttacking = false;

    [Header("Combat Memory & Aggro")]
    [Tooltip("Distanza massima prima che il nemico si arrenda e perda l'aggro")]
    [SerializeField] float loseCombatRange = 15f;
    private bool isInCombat = false;

    [Header("Stagger Settings")]
    [SerializeField] float staggerDuration = 2f;
    private float staggerEndTime;
    private bool isStaggered = false;

    private WeaponHitbox weaponHitbox;

    private EnemyHealth enemyHealth;
    private NavMeshAgent agent;
    Animator anim;

    // Macchina a stati semplificata con Enum per il comportamento del nemico
    private enum EnemyState { Patrol, Chasing, Attacking, Defending, Recovering, Staggered}
    private EnemyState currentState = EnemyState.Patrol;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        enemyHealth = GetComponent<EnemyHealth>();

        weaponHitbox = GetComponentInChildren<WeaponHitbox>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (isStaggered)
        {
            if (Time.time < staggerEndTime)
            {
                currentState = EnemyState.Staggered;
                StaggeredBehavior();
                return;
            }

            EndStagger();
        }

        // --- 1. I LUCCHETTI (Proteggono l'azione in esecuzione) ---
        if (isAttacking)
        {
            currentState = EnemyState.Attacking;
            AttackingBehavior();
            return;
        }
        
        if (Time.time < endRecoveryTime)
        {
            currentState = EnemyState.Recovering;
            RecoveringBehavior();
            return;
        }

        // --- 2. LA MEMORIA (Aggro Lock) ---

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (CanSeePlayer())
        {
            isInCombat = true;            
        }

        if (isInCombat && distanceToPlayer > loseCombatRange)
        {
            isInCombat = false;
            Debug.Log("Il giocatore è fuggito");
        }

        // --- 3. IL CERVELLO (Decide il prossimo stato basandosi SULLA MEMORIA) ---
        if (isInCombat)
        {
            float currentAttackThreshold = (currentState == EnemyState.Defending || currentState == EnemyState.Recovering) ? attackRange + 0.5f : attackRange;

            if (distanceToPlayer <= currentAttackThreshold)
            {
                if (Time.time >= nextAttackTime)
                {
                    currentState = EnemyState.Attacking;
                }
                else
                {
                    currentState = EnemyState.Defending;
                }
            }
            else
            {
                currentState = EnemyState.Chasing;
            }
        }        
        else
        {
            currentState = EnemyState.Patrol;
        }


            switch (currentState)
            {
                case EnemyState.Patrol:
                    PatrolBehavior();
                    break;
                case EnemyState.Chasing:
                    ChasingBehavior();
                    break;
                case EnemyState.Defending:
                    DefendingBehavior();
                    break;
                case EnemyState.Recovering:
                    RecoveringBehavior();
                    break;
                case EnemyState.Attacking:
                    AttackingBehavior();
                    break;
            }

        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(agent.velocity, Vector3.up);
        anim.SetFloat("Speed", horizontalVelocity.magnitude);
    }

    private bool CanSeePlayer()
    {
        // PRIMO CONTROLLO (ESISTE IL PLAYER?)
        if(player == null) return false;

        // SECONDO CONTROLLO (IL PLAYER È NEL RAGGIO DI AZIONE?)
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if(distanceToPlayer > chaseRange) return false; // Il giocatore è fuori dal raggio di inseguimento       

        // TERZO CONTROLLO (IL PLAYER È NEL CAMPO VISIVO?)
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle / 2f) return false;

        // QUARTO CONTROLLO (CI SONO OSTACOLI TRA IL NEMICO E IL PLAYER?)
        Vector3 rayStart = transform.position + Vector3.up; // Alza il punto di partenza del raycast per evitare collisioni con il terreno
        Vector3 rayDir = ((player.position + Vector3.up) - rayStart).normalized; // Punta verso il centro del giocatore

        Debug.DrawRay(rayStart, rayDir * distanceToPlayer, Color.red); // Disegna il raggio per il debug

        if (Physics.Raycast(rayStart, rayDir, out RaycastHit hit, distanceToPlayer, obstacleLayer))
        {
            // Se il raycast colpisce qualcosa che non sia il PLayer, allore è un muro!
            if(!hit.collider.CompareTag("Player")) return false;
        }

        return true; // Il nemico non può vedere il giocatore
    }

    private void PatrolBehavior()
    {
        if(waypoints.Length == 0) return;

        if(isWaiting) return;

        anim.SetBool("IsDefending", false);
        if(enemyHealth != null) enemyHealth.isDefending = false;

        agent.isStopped = false;
        agent.speed = patrolSpeed;

        agent.SetDestination(waypoints[currentWaypointIndex].position);


        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private void ChasingBehavior()
    {
        anim.SetBool("IsDefending", false);
        if (enemyHealth != null) enemyHealth.isDefending = false;

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void DefendingBehavior()
    {
        agent.isStopped = true;
        anim.SetBool("IsDefending", true);

        if (enemyHealth != null) enemyHealth.isDefending = true;


        FacePlayerSmoothly();
    }

    private void RecoveringBehavior()
    {
        agent.isStopped = true;
        anim.SetBool("IsDefending", false);

        if (enemyHealth != null) enemyHealth.isDefending = false;
    }

    private void FacePlayerSmoothly()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Mantieni l'orientamento solo sull'asse Y

        if(direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void AttackingBehavior()
    {
        agent.isStopped = true;
        if (!isAttacking)
        {
            isAttacking = true;

            if (enemyHealth != null) enemyHealth.isDefending = false;

            anim.SetTrigger("Attack");

            Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookPos);

            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        endRecoveryTime = Time.time + recoveryTime;
    }

    public void Stagger()
    {
        if (isStaggered) return;

        isStaggered = true;
        staggerEndTime = Time.time + staggerDuration;

        // 1. INTERRUZIONE
        isAttacking = false;
        if(weaponHitbox != null) weaponHitbox.DisableHitbox();
        anim.ResetTrigger("Attack");

        // 2. BLOCCO DELL'IA
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // 3. VULNERABILITà
        if (enemyHealth != null) enemyHealth.isDefending = false;
        anim.SetBool("IsDefending", false);

        // 4. ANIMAZIONE
        anim.SetBool("IsStaggered", true);
        anim.SetTrigger("Stagger");

        Debug.Log("Il cavaliere è STORDITO!");
    }

    private void StaggeredBehavior()
    {
        agent.isStopped = true;
        if (enemyHealth != null) enemyHealth.isDefending = false;
    }
    private void EndStagger()
    {
        isStaggered = false;
        anim.SetBool("IsStaggered", false );
        Debug.Log("Il cavaliere si riprende");
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTimeAtWaypoint);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }

    private void Attack()
    {
        Vector3 lookPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPosition);

       
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizza il raggio di inseguimento e attacco nella scena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Raggio di fuga / Aggro (loseCombatRange)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, loseCombatRange);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    /*
     * 
     * PARRY RIUSCITO (La weaponhitbox del nemico riceve Parried) --> owner.GetComponent<IStraggerable>().Stagger(): 
     * 
     * - Codice EnemyAI: Timer di stagger + luccheto che blocca l'update
     * - Animator: Trigger "Stagger" -> Impact (una volta) -> Dizzy (loop)
     
     
     */
}
