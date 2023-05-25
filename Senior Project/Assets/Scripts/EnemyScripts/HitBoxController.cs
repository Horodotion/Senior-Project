using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    //Damage value is been change in the melee attacks
    [SerializeField]  BossEnemyController bossEnemyController;
    [HideInInspector] public float damage;

    private void Awake()
    {
        bossEnemyController = GetComponent<BossEnemyController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerController.puppet.ChangeTemperature(damage);
            this.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (bossEnemyController.bossState != BossState.meleeAttack)
        {
            this.gameObject.SetActive(false);
        }
    }
}
