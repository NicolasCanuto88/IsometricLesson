using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeWall : MonoBehaviour
{
    //Cache pubblica per i renderer dei muri compositi, da assegnare nell'Inspector
    public Renderer[] groupRenderers; // Array di renderer dei muri compositi

    private void Awake()
    {
        groupRenderers = GetComponentsInChildren<Renderer>(); // Otteniamo tutti i renderer dei figli
    }
}
