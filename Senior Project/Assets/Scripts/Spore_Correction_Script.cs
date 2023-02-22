using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Spore_Correction_Script : MonoBehaviour
{
    public GameObject sporeEffect;

    private void Awake()
    {
        sporeEffect.transform.Rotate ((-this.transform.rotation.x*100),0,0);
        sporeEffect.transform.lossyScale.Set(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
    }
}
