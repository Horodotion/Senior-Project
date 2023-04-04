using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TMP_Text damageTextMesh;
    public Material ourMaterial;
    public Animator damageTextAnimator;
    public float damageAmount;

    void Awake()
    {
        damageTextMesh = GetComponentInChildren<TMP_Text>();
        damageTextAnimator = GetComponent<Animator>();
    }

    public void UpdateDamage(Vector3 screenPosition, float damage)
    {
        damageAmount += damage;

        if (damage > 0)
        {
            damageTextMesh.text = damage.ToString();
        }
        else
        {
            damageTextMesh.text = "immune";
        }
        
    }

    public void TurnOffDamageText()
    {
        damageAmount = 0;
        damageTextMesh.text = "immune";
        gameObject.SetActive(false);
    }
}
