using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneScript : MonoBehaviour
{
    public int connectingScene; // Which scene the loading zone connects to
    public int connectingZone; // The ID for which sone in the connecting scene to load
    public bool zoneActive = false; // A bool to set whether or not to let the loading zone be activatable or not

    // Once the player enters a loading zone, it checks if it's tagged as player and that the zone is active
    // it sets the connecting zone's ID number and loads the corresponding scene
    void OnTriggerEnter2D(Collider2D col)
    {
    
        if (col.gameObject.tag == "Player" && zoneActive == true)
        {
            LoadingZoneManager.zoneToLoad = connectingZone;
            // GeneralManager.LoadZone(connectingScene);
        }
    }
    
    // Once the Player exits the loading zone, it sets the zone to be activatable
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && zoneActive == false)
        {
            zoneActive = true;
        }
    }
}
