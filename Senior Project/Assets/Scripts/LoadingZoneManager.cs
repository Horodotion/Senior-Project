using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneManager : MonoBehaviour
{
    public static int zoneToLoad; // A static reference to which zone to load
    public List<GameObject> zones; // A list of gameObjects to act as the loading zones

    // At the start of a scene
    // This Manager sets the zone's activation criteria to false
    // Then it sets the player's position to the zone in question
    void Start()
    {
        // if (PlayerController.ourPlayer != null)
        // {
        //     zones[zoneToLoad].GetComponent<LoadingZoneScript>().zoneActive = false;
        //     PlayerController.ourPlayer.gameObject.transform.position = zones[zoneToLoad].transform.position;

        //     Camera.main.transform.position = new Vector3(zones[zoneToLoad].transform.position.x, zones[zoneToLoad].transform.position.y, -10);
        // }
    }
}