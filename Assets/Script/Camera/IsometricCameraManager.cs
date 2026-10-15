using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IsometricCameraManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CinemachineVirtualCamera vCam;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f; // Distanza minima della camera
    [SerializeField] private float maxZoom = 15f; // Distanza massima della camera
    [SerializeField] private float zoomSpeed = 10f; // Velocità di scroll
    [SerializeField] private float smoothSpeed = 10f; // MOrbidezza della frenata


    private float targetZoom; // Distanza target della camera
    // Start is called before the first frame update
    void Start()
    {
        if (vCam == null) vCam = GetComponent<CinemachineVirtualCamera>();

        // Partiamo dallo zoom iniziale della camera
        targetZoom = vCam.m_Lens.OrthographicSize;

        // Setup opzionale: Segui il player automaticamente
        if(vCam.Follow == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if(player != null)
            {
                vCam.Follow = player.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 1. Leggi Input
        // ReadValue restituisce la direzione dello scroll (positiva o negativa)
        float scroll = InputManager.Instance.Zoom.ReadValue<float>();

        // 2. Calcola Target
        if (scroll != 0)
        {
            //Normalizziamo lo scroll in un intervallo più gestibile e lo applichiamo al target
            targetZoom -= scroll * zoomSpeed * Time.deltaTime * 0.001f; // Modifichiamo il target in base allo scroll

            // Blocchiamo il valore tra min e max
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // 3. Applica lo zoom fluido
        //Spostiamo il valore attuale della distanza verso il target usando MoveToward, in modo fluido
        vCam.m_Lens.OrthographicSize = Mathf.MoveTowards(vCam.m_Lens.OrthographicSize, targetZoom, smoothSpeed * Time.deltaTime);
    }
}
