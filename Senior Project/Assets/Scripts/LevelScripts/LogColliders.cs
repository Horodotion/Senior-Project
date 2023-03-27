using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogColliders : MonoBehaviour
{
    public GameObject boxCol;
    public GameObject meshCol;
    public bool falling = false;
    public int vinesLeft;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCol.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(vinesLeft <= 0)
        {
            falling = true;
        }
        
        if(falling)
        {
            meshCol.SetActive(false);
            boxCol.SetActive(true);
        }
    }
}
