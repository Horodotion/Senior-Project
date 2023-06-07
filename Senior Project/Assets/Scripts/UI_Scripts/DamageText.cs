using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TMP_Text damageTextMesh;
    public Vector3 worldPosition;
    public EnemyController ourEnemy;
    public Animator damageTextAnimator;
    public float damageAmount;

    void Awake()
    {
        damageTextMesh = GetComponentInChildren<TMP_Text>();
        damageTextAnimator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
    }

    public void UpdateDamage(Vector3 screenPosition, float damage, DamageType damageType = DamageType.nuetral)
    {
        damageAmount += damage;
        damageTextAnimator.ResetTrigger("Activate");
        damageTextAnimator.SetTrigger("Activate");
        
        if (damageAmount >= 0)
        {
            damageTextMesh.text = System.Math.Round(damageAmount).ToString();
        }
        else
        {
            damageTextMesh.text = "immune";
        }

        if (damageType == DamageType.fire)
        {
            damageTextMesh.font = SpawnManager.instance.fireDamageFont;
            damageTextMesh.color = SpawnManager.instance.fireDamageColor;
        }
        else if (damageType == DamageType.ice)
        {
            damageTextMesh.font = SpawnManager.instance.iceDamageFont;
            damageTextMesh.color = SpawnManager.instance.iceDamageColor;
        }
        
        worldPosition = screenPosition;
    }

    public void TurnOffDamageText()
    {
        damageAmount = 0;
        damageTextMesh.text = "immune";
        gameObject.SetActive(false);

        if (ourEnemy != null)
        {
            if (ourEnemy.fireDamageText != null && ourEnemy.fireDamageText == gameObject)
            {
                ourEnemy.fireDamageText = null;
            }
            else if (ourEnemy.iceDamageText != null && ourEnemy.iceDamageText == gameObject)
            {
                ourEnemy.iceDamageText = null;
            }
            
            ourEnemy = null;
        }
    }
}
