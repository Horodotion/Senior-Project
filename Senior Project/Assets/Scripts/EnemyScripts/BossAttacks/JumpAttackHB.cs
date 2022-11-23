using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackHB : MonoBehaviour
{
    private float duration;
    private float damage;
    private bool isFirstTime = true;
    private Vector3 startingPosition;

    public void SetUp(float duration, float damage)
    {
        this.duration = duration;
        this.damage = damage;
        isFirstTime = true;
        startingPosition = transform.position;
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && isFirstTime)
        {
            PlayerController.puppet.Damage(this.damage);
            isFirstTime = false;
            
        }
        
    }
}
