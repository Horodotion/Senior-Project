using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDisintegration : MonoBehaviour
{
    [Header("References")]
    public GameObject toAppear;
    public Renderer rend;
    private float shaderValue;
    private bool playerIn = false;

    [Header("Settings")]
    private float startValue = 0;
    private float endValue = 100;
    public float rateOfThing = 2;
    public float changeValue = .1f;

    // Start is called before the first frame update
    void Start()
    {
        toAppear.SetActive(false);
        rend = toAppear.GetComponent<Renderer>();
        shaderValue = rend.material.GetFloat("_Disintigration");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIn)
        {
            if (shaderValue > 0)
            {
                toAppear.SetActive(true);
                shaderValue -= changeValue * (rateOfThing * Time.deltaTime);
                Debug.Log(shaderValue);
                rend.material.SetFloat("_Disintigration", shaderValue);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerIn = true;
        }
    }
}
