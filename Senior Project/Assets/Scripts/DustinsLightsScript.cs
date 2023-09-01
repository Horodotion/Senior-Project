using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DustinsLightsScript : MonoBehaviour
{
    public static DustinsLightsScript instance;
    public static float currentBrightness;
    public Volume volume;
    public LiftGammaGain gammaGain;
    public Vignette vignette;

    void Awake()
    {
        instance = this;
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out gammaGain);

        SetGain(currentBrightness);
    }


    public static void SetGain(float newGain)
    {
        currentBrightness = newGain;
        Vector4 tempVector = instance.gammaGain.gain.value;
        tempVector.w = currentBrightness;
        instance.gammaGain.gain.Override(tempVector);

        currentBrightness = newGain;
    }
}
