using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//To utilize the VFX you must have UnityEngine.VFX
using UnityEngine.VFX;

public class VFX_Texting_Script : MonoBehaviour
{

    //public variables for visual effect
    public VisualEffect flamethrower;
    public VisualEffect shotgun;


    // Start is called before the first frame update
    void Start()
    {
        //Stop the flamethrower effect
        flamethrower.Stop();
    }

    // Update is called once per frame
    void Update()
    {    
        if (Input.GetKeyDown(KeyCode.E))
        {
            //This is the command to play the flamethrower effect
            flamethrower.Play();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            //This is the command to stop the flamethrower effect
            flamethrower.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //This is the command to play the shotgun effect
            shotgun.Play();
        }
    }
}
