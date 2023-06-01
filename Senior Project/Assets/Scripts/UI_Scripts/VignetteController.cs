using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VignetteController : MonoBehaviour
{
    public string intensityName = "_Intensity";
    public float intensity;
    // public Material ourMaterial;
    public Image ourImage;

    void Awake()
    {
        // ourMaterial = GetComponent<Image>().material;
        ourImage = GetComponent<Image>();
    }

    public void SetVignetteIntensity(float newIntensity)
    {
        intensity = newIntensity;
        // ourMaterial.SetFloat(intensityName, intensity);

        ourImage.material.SetFloat(intensityName, intensity);
    }
}
