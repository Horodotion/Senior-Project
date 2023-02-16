using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLight : TriggerScript
{
    public static int currentActivePath;
    public static Dictionary<int, PathLight> allLights = new Dictionary<int, PathLight>();
    public int ourPathID;
    public GameObject lightToTurnOff;

    void Awake()
    {
        allLights[ourPathID] = this;
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
            return;
        }

        currentActivePath = ourPathID + 1;

        for (int i = 0; i < currentActivePath; i++)
        {
            if (allLights[i].lightToTurnOff.activeInHierarchy)
            {
                allLights[i].lightToTurnOff.SetActive(false);
            }
        }
        
        if (allLights.ContainsKey(currentActivePath))
        {
            allLights[currentActivePath].lightToTurnOff.SetActive(true);
        }
    }
}
