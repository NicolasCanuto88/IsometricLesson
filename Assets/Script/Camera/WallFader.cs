using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFader : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    // Stato
    private float currentHoleActivity = 0f;
    private float targetHoleActivity = 0f;
    private float autoCloseTimer = 0f;

    // Parametri configurabili (visivi)
    private float currentRadius = 1f;
    private float currentDensity = 30f;
    private float openSpeed = 10f;
    private float closeSpeed = 10f;

    // Cache degli ID dello Shader
    private static int HoleActiveID = Shader.PropertyToID("_HoleActive");
    private static int PlayePosID = Shader.PropertyToID("_PlayerPos");
    private static int HoleRadiusID = Shader.PropertyToID("_HoleRadius");
    private static int PixelDensityID = Shader.PropertyToID("_PixelDensity");

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (_renderer == null) return;

        // 1. Logica Timer: Decidiamo se aprire o chiudere il buco
        autoCloseTimer -= Time.deltaTime;
        targetHoleActivity = (autoCloseTimer > 0f) ? 1f : 0f;

        // 2. Animazione: Scegliamo la velocità giusta
        float smoothSpeed = (targetHoleActivity > 0.5) ? openSpeed : closeSpeed;
        currentHoleActivity = Mathf.MoveTowards(currentHoleActivity, targetHoleActivity, smoothSpeed * Time.deltaTime);

        // 3. Applichiamo alla GPU
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(HoleActiveID, currentHoleActivity);
        _propBlock.SetFloat(HoleRadiusID, currentRadius);
        _propBlock.SetFloat(PixelDensityID, currentDensity);

        _renderer.SetPropertyBlock(_propBlock);
    }

    public void StayOpen(Vector3 pos, float speedOpen, float speedClose, float radius, float density)
    {
        autoCloseTimer = 0.1f; // Resetta il timer per chiudere automaticamente

        // Salviamo i dati passati dalla Camera
        openSpeed = speedOpen;
        closeSpeed = speedClose;
        currentRadius = radius;
        currentDensity = density;

        if (_renderer != null)
        {
            // Aggiorniamo SUBITO la posizione per evitare lag visivo nel buco
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetVector(PlayePosID, pos);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
