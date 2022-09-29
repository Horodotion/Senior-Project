using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEvent : MonoBehaviour
{
    [HideInInspector] HitScanEnemyController HSEController;
    [SerializeField] GameObject navMeshPoint;
    private void Awake()
    {
        HSEController = navMeshPoint.GetComponent<HitScanEnemyController>();
    }

    public void FireHitScanAni()
    {
        HSEController.FireHitScanAni();
    }
    public void FireHitScan()
    {
        HSEController.FireHitScan();
    }

    public void LowerArms()
    {
        HSEController.LowerArms();
    }

    public void ReturnToChasing()
    {
        HSEController.ReturnToChasing();
    }

}
