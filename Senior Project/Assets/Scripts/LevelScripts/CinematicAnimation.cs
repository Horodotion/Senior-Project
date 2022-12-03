using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicAnimation : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private float bossLifetime;
    private float timer;
    private bool dying = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            bossAnimator.SetBool("playerIn", true);
            dying = true;
            Debug.Log("startTimer");
        }
    }

    private void Update()
    {
        if (dying == true)
        {
            timer += Time.deltaTime;
            if (timer >=  bossLifetime)
            {
                boss.gameObject.SetActive(false);
                Debug.Log("I died");
            }
        }
    }
}
