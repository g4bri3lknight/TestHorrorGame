//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_CameraBloodEffect : MonoBehaviour {

    // Inspector Assigned
    [SerializeField]
    private Texture2D _bloodTexture = null;

    [SerializeField]
    private Texture2D _bloodNormalMap = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _bloodAmount = 0.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _distortion = 1.0f;

    [SerializeField]
    private Shader _shader = null;

    // Private
    private Material _material = null;

    // Properties
    public float bloodAmount { get { return _bloodAmount; } set { _bloodAmount = value; } }

    HDK_PlayerHealth healthScript;

    void Start()
    {
        healthScript = GetComponentInParent<HDK_PlayerHealth>();
    }

    void Update()
    {
        float difference = healthScript.maxHealth - healthScript.Health;
        bloodAmount = difference / 100f;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_shader == null) return;
        if (_material == null)
        {
            _material = new Material(_shader);
        }
        if (_material == null) return;
        if (_bloodTexture != null)
            _material.SetTexture("_BloodTex", _bloodTexture);

        if (_bloodNormalMap != null)
            _material.SetTexture("_BloodBump", _bloodNormalMap);

        _material.SetFloat("_Distortion", _distortion);
        _material.SetFloat("_BloodAmount", _bloodAmount);
        Graphics.Blit(src, dest, _material);
    }
}