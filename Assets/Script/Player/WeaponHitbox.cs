using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] int attackDamage = 10; // Danno base dell'attacco

    [SerializeField] LayerMask targetLayer; // Layer dei nemici da colpire

    private BoxCollider weaponCollider; // Riferimento al BoxCollider

    private List<Collider> alreadyHitEnemies = new List<Collider>(); // Lista per tenere traccia dei nemici già colpiti

    [SerializeField] float hitStopDuration = 0.1f; // Durata del hit stop in secondi
    [SerializeField] float hitStopTimeScale = 0.1f; // Scala del tempo durante il hit stop

    private CinemachineImpulseSource impulseSource; // Riferimento al Cinemachine Impulse Source

    [SerializeField] private GameObject owner;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>();
        weaponCollider.isTrigger = true; // Assicurati che il BoxCollider sia un trigger
        weaponCollider.enabled = false; // Disabilita il collider all'inizio

        impulseSource = GetComponent<CinemachineImpulseSource>();

    }
    
    public void EnableHitbox()
    {
        weaponCollider.enabled = true; // Abilita il collider quando l'attacco è attivo
        alreadyHitEnemies.Clear(); // Pulisci la lista dei nemici colpiti
    }
    public void DisableHitbox() 
    {
        weaponCollider.enabled = false; // Disabilita il collider quando l'attacco è finito
    }

    private IEnumerator HitStopRoutine()
    {
        if (Time.timeScale == 0) yield break; // Se siamo già in hit stop, esci

        Time.timeScale = hitStopTimeScale; // Rallenta il tempo per un breve momento

        yield return new WaitForSecondsRealtime(hitStopDuration); // Attendi un breve periodo di tempo reale

        if(Time.timeScale != 0)
            Time.timeScale = 1f; // Ripristina il tempo normale
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHitEnemies.Contains(other)) return; // Se il nemico è già stato colpito, esci

        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0) return; // Controlla se l'oggetto colpito è un nemico


        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if(impulseSource != null)
            {
                 impulseSource.GenerateImpulse(); // Genera un impulso per la camera shake
            }

            DamageResult result = damageable.TakeDamage(attackDamage, owner); // Applica il danno al player e ottieni il risultato
            Debug.Log($"Esito colpo: {result}"); // Log dell'esito del colpo

            if(result == DamageResult.Parried && owner != null)
            {
                IStaggerable staggerable = owner.GetComponent<IStaggerable>();
                if (staggerable != null) staggerable.Stagger();
            }

            alreadyHitEnemies.Add(other); // Aggiungi il nemico alla lista dei colpiti

            StartCoroutine(HitStopRoutine()); // Avvia la routine del hit stop
        }
    }
}
