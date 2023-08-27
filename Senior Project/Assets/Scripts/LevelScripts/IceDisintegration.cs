using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDisintegration : MonoBehaviour
{
    [Header("References")]
    public GameObject iceWall;
    public Renderer rend;
    private float shaderValue;
    private bool disintegrate = false;
    [SerializeField] private Transform destination;
    [SerializeField] private float speed;

    [Header("Settings")]
    private float startValue = 0;
    private float endValue = 100;
    public float rateOfThing = 2;
    public float changeValue = .1f;


    // Start is called before the first frame update
    void Start()
    {
        rend = iceWall.GetComponent<Renderer>();
        shaderValue = rend.material.GetFloat("_Disintigration");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (disintegrate)
        {
            Debug.Log("I'm doing it!");
            
            if (shaderValue < 1)
            {
                shaderValue += changeValue * (rateOfThing * Time.deltaTime);
                Debug.Log(shaderValue);
                rend.material.SetFloat("_Disintigration", shaderValue);
            }

            iceWall.transform.position = Vector3.MoveTowards(iceWall.transform.position, destination.position, speed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            disintegrate = true;
        }
    }
}
