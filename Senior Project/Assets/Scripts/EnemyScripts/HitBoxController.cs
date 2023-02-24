using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    //Damage value is been change in the melee attacks
    [HideInInspector] public float damage;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerController.puppet.ChangeTemperature(damage);
            this.gameObject.SetActive(false);
        }
    }
}
