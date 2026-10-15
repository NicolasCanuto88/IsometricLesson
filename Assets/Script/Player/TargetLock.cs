using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TargetLock : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lockRadius = 15f;
    [SerializeField] private LayerMask enemyLayer;

    public Transform currentTarget;
    public bool isLockedOn = false;


    private void OnEnable()
    {
        if(InputManager.Instance != null) InputManager.Instance.OnLockOn += ToggleLockOn;
    }

    private void OnDisable()
    {
        if(InputManager.Instance != null) InputManager.Instance.OnLockOn -= ToggleLockOn;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLockedOn)
        { 
            if(currentTarget == null || !currentTarget.gameObject.activeInHierarchy || Vector3.Distance(transform.position, currentTarget.position) > lockRadius) 
            {
                isLockedOn = false; // Se il bersaglio è stato distrutto, disattiva il lock-on
                currentTarget = null;
            }
        }
    }

    private void ToggleLockOn()
    {
        if(isLockedOn)
        {
            isLockedOn = false;
            currentTarget = null;
        }
        else
        {
            FindClosestEnemy();
        }
    }

    private void FindClosestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, lockRadius, enemyLayer);
        if(enemiesInRange.Length == 0) return; // Se non ci sono nemici, esci

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if(dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null) 
        {
            currentTarget = closestEnemy;
            isLockedOn = true;
        }
    }
}
