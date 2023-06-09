using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VignetteController : MonoBehaviour
{
    public string intensityName = "_Intensity";
    public float intensity;
    public Image ourImage;

    void Awake()
    {
        ourImage = GetComponent<Image>();
    }

    public void SetVignetteIntensity(float newIntensity)
    {
        intensity = newIntensity;
        ourImage.material.SetFloat(intensityName, intensity);
    }
}
