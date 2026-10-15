using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OutlineController : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 0.1f)]
    public float outlineWidth = 0.005f;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;


    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }


    // Update is called once per frame
    void Update()
    {
        if (_renderer == null) return;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Thickness", outlineWidth);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
