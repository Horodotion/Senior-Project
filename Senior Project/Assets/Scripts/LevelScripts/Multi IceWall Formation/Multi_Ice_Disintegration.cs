using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_Ice_Disintegration : MonoBehaviour
{
    [Header("References")]
    public IceFormationManager manager;
    [SerializeField] private GameObject[] iceWallsToAppear;
    public GameObject currentIceWall;
    public Renderer rend;
    [SerializeField] private float shaderValue;
    private bool playerIn = false;
    private int currentIndex = 0;

    [Header("Settings")]
    private float startValue = 0;
    private float endValue = 100;
    public float rateOfThing = 2;
    public float changeValue = .1f;

    // Start is called before the first frame update
    void Start()
    {
        if(manager == null)
        {
            manager = FindObjectOfType<IceFormationManager>();
        }
        iceWallsToAppear = manager.iceToForm;
        currentIceWall = iceWallsToAppear[currentIndex];
        //currentIceWall.SetActive(false);
        rend = currentIceWall.GetComponent<Renderer>();
        shaderValue = rend.material.GetFloat("_Disintigration");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIn)
        {
            if (shaderValue > 0)
            {
                currentIceWall.SetActive(true);
                shaderValue -= changeValue * (rateOfThing * Time.deltaTime);
                Debug.Log(shaderValue);
                rend.material.SetFloat("_Disintigration", shaderValue);
            }
            if (shaderValue <= 0)
            {
                IterateIceWallIndex();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIn = true;
        }
    }

    private void IterateIceWallIndex()
    {
        if(currentIndex == iceWallsToAppear.Length)
        {
            return;
        }
        else
        {
            currentIndex++;
            currentIceWall = iceWallsToAppear[currentIndex];
            currentIceWall.SetActive(true);
            rend = currentIceWall.GetComponent<Renderer>();
            shaderValue = rend.material.GetFloat("_Disintigration");
        }
    }
}
