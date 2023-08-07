using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JeffSpawner : MonoBehaviour
{
    [SerializeField] private GameObject jeff;
    [SerializeField] private bool isIn;
    
    // Start is called before the first frame update
    void Start()
    {
        jeff.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            jeff.SetActive(true);
        }
    }
}
