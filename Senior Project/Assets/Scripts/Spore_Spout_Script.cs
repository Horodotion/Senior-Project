using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Spore_Spout_Script : MonoBehaviour
{
    public GameObject spoutEffect;
    private GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            spoutEffect.GetComponent<VisualEffect>().Play();
        }
    }
}
