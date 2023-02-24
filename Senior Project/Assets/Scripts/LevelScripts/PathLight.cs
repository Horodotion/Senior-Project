using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLight : TriggerScript
{
    public static int currentActivePath;
    public static Dictionary<int, PathLight> allLights = new Dictionary<int, PathLight>();
    public int ourPathID;
    public Light ourLight;
    [HideInInspector] public float baseBrightness;
    public float maxIntensityMultiplier;
    [HideInInspector] public float maxIntensity;
    public float minIntensityMultiplier;
    [HideInInspector] public float minIntensity;
    public float fadeSpeed;

    void Awake()
    {
        allLights[ourPathID] = this;
        if (GetComponent<Light>() != null)
        {
            ourLight = GetComponent<Light>();
        }
        else if (GetComponentInChildren<Light>() != null)
        {
            ourLight = GetComponentInChildren<Light>();
        }

        baseBrightness = ourLight.intensity;
        maxIntensity = baseBrightness * maxIntensityMultiplier;
        minIntensity = baseBrightness * minIntensityMultiplier;

    }

    public static void ClearPath()
    {
        allLights.Clear();
        currentActivePath = 0;
    }

    public override void ActionToTrigger()
    {
        if (ourPathID < currentActivePath)
        {
            for (int i = 0; i > 0; i--)
            {
                allLights[i].StopAllCoroutines();
                allLights[i].ChangeLightIntensity(allLights[i].ourLight, allLights[i].baseBrightness);
            }
        }

        currentActivePath = ourPathID + 1;

        for (int i = 0; i < currentActivePath; i++)
        {
            allLights[i].StopAllCoroutines();
            allLights[i].StartCoroutine(ChangeLightIntensity(allLights[i].ourLight, allLights[i].minIntensity));
        }
        
        if (allLights.ContainsKey(currentActivePath))
        {
            allLights[currentActivePath].StopAllCoroutines();
            allLights[currentActivePath].StartCoroutine(ChangeLightIntensity(allLights[currentActivePath].ourLight, allLights[currentActivePath].maxIntensity));
        }
    }

    public override void ActionToStop()
    {
        if (allLights.ContainsKey(currentActivePath))
        {
            allLights[currentActivePath].StopAllCoroutines();
            allLights[currentActivePath].StartCoroutine(ChangeLightIntensity(allLights[currentActivePath].ourLight, allLights[currentActivePath].baseBrightness)); 
        }
    }

    public IEnumerator ChangeLightIntensity(Light light, float targetIntensity)
    {
        while (light.intensity != targetIntensity)
        {
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.fixedDeltaTime * fadeSpeed);
            yield return null;
        }
    }
}
