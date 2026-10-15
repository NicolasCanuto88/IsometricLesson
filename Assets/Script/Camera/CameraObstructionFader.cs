using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionFader : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform player;
    [SerializeField] LayerMask wallLayer;

    [Header("Animation Settings")]
    [SerializeField] float heightOffset = 1.5f;
    [SerializeField] float fadeSpeed = 10f;
    [SerializeField] float closeSpeed = 10f;
    [SerializeField] float rayRadius = 0.5f;

    [Header("Visual Style")]
    [SerializeField] float holeRadius = 1.5f;
    [SerializeField] float pixelDensity = 30f;


    // Debug Visivo
    private Vector3 debugRayStart;
    private Vector3 debuRayEnd;
    private bool isHitDebug;

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + Vector3.up * heightOffset;
        Vector3 dir = targetPos - transform.position;
        float dist = dir.magnitude;

        debugRayStart = transform.position;
        debuRayEnd = targetPos;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, rayRadius, dir, dist, wallLayer);
        isHitDebug = hits.Length > 0;

        foreach (RaycastHit hit in hits)
        {
            CompositeWall group = hit.collider.GetComponentInParent<CompositeWall>();

            if(group != null)
            {
                foreach (Renderer rend in group.groupRenderers)
                {
                    HandleFader(rend.gameObject, targetPos);
                }
            }
            else
            {
                HandleFader(hit.collider.gameObject, targetPos);
            }
        }
    }

    void HandleFader(GameObject obj, Vector3 targetPos)
    {
        WallFader fader = obj.GetComponent<WallFader>();
        if(fader == null) fader = obj.AddComponent<WallFader>();

        //Passiamo i parametri
        fader.StayOpen(targetPos, fadeSpeed, closeSpeed, holeRadius, pixelDensity);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isHitDebug ? Color.red : Color.green;
        Gizmos.DrawLine(debugRayStart, debuRayEnd);
        Gizmos.DrawWireSphere(debuRayEnd, rayRadius);
    }
}
