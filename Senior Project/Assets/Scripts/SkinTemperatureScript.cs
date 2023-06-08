using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinTemperatureScript : MonoBehaviour
{
    public string intensityName = "_HeightAmount";
    // public float intensity;
    public Material ourMaterial;
    public Renderer ourRenderer;

    void Awake()
    {
        ourRenderer = GetComponent<Renderer>();
    }

    public void SetShaderIntensity(float newIntensity)
    {
        // intensity = newIntensity;
        ourRenderer.material.SetFloat(intensityName, newIntensity);
    }
}

