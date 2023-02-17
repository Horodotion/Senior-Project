using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashLightTriggers : MonoBehaviour
{
    public GameObject pathLight;
    public float baseIntensity;
    [SerializeField] private float intensityMultiplier = 1.2f;
    public float maxIntensity;
    public float fadeSpeed = 1f;
    

    private void Start()
    {
        baseIntensity = pathLight.GetComponent<Light>().intensity;
        maxIntensity = baseIntensity * intensityMultiplier;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //pathLight.GetComponent<Light>().intensity = baseIntensity * intensityMultiplier;
            pathLight.GetComponent<Light>().intensity = Mathf.Lerp(maxIntensity, baseIntensity, Time.deltaTime * fadeSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //pathLight.GetComponent<Light>().intensity = baseIntensity;
            pathLight.GetComponent<Light>().intensity = Mathf.Lerp(baseIntensity, maxIntensity, Time.deltaTime * fadeSpeed);
        }
    }
}
