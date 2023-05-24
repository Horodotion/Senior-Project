using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [HideInInspector] public float laserFrequency;
    [HideInInspector] public float laserDamage;
    [HideInInspector] private float timer;
    [SerializeField] float maxlaserLength;
    [SerializeField] bool isInfiniteSpeed;
    [SerializeField] float laserSpeed;
    [SerializeField] private float laserSize = 1;
    [SerializeField] GameObject[] laserRayCastPoints;
    [SerializeField] LayerMask layer;
    private float currentLaserLegth;

    private void Awake()
    {
        laserSize = laserSize * transform.localScale.z;
    }
    void OnEnable()
    {
        this.transform.localScale = new Vector3(laserSize, laserSize, .1f);
    }

    void Update()
    {
        if (isInfiniteSpeed)
        {
            if (PointsThatHit(this.transform.localScale.z, layer) > 0)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, GetLongestDistanceFromPointsHit(this.transform.localScale.z, layer));
                return;
            }

            if (PointsThatHit(this.transform.localScale.z, layer) == 0)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, maxlaserLength);
                return;
            }

        }
        else
        {
            if (PointsThatHit(this.transform.localScale.z, layer) > 0)
            {
                float temp = GetLongestDistanceFromPointsHit(maxlaserLength, layer);
                if (temp < this.transform.localScale.z)
                {
                    this.transform.localScale = new Vector3(laserSize, laserSize, temp);
                }
                currentLaserLegth = temp;
            }

            if (PointsThatHit(this.transform.localScale.z, layer) == 0)
            {
                currentLaserLegth = maxlaserLength;
            }


            if (currentLaserLegth > this.transform.localScale.z)
            {
                this.transform.localScale += new Vector3(0, 0, laserSpeed * Time.deltaTime);
            }
        }

    }


    //Damage value is been change in the attacks
    private void OnTriggerStay(Collider other)
    {
        
        timer -= Time.deltaTime;
        Debug.Log(timer <= 0);
        if (timer <= 0)
        {
            
            if (other.tag.Equals("Player"))
            {
                
                PlayerController.puppet.ChangeTemperature(laserDamage);
                timer = laserFrequency;
            }
        }
        else
        {
            timer = 0;
        }
        
    }

    private int PointsThatHit(float rayCastDistance, LayerMask thatLayer)
    {
        int temp = 0;
        //Debug.Log(laserRayCastPoints.Length);
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? maxlaserLength : rayCastDistance, thatLayer);
            //Debug.Log(hit.collider == null);
            if (hit.collider != null)
            {
                temp++;
            }
        }
        //Debug.Log(temp);
        return temp;
    }


    private float GetLongestDistanceFromPointsHit(float rayCastDistance, LayerMask thatLayer)
    {
        
        RaycastHit[] rayCastThatHit = new RaycastHit[laserRayCastPoints.Length];
        
        for(int i = 0; i < laserRayCastPoints.Length; i++)
        {
            Physics.Raycast(laserRayCastPoints[i].transform.position, laserRayCastPoints[i].transform.forward, out RaycastHit hit,isInfiniteSpeed? maxlaserLength : rayCastDistance, thatLayer);
            rayCastThatHit[i] = hit;
            
        }

        return FindTheLongestDistanceOnTheSameObject(rayCastThatHit);
    }

    private float FindTheLongestDistanceOnTheSameObject(RaycastHit[] rayCastThatHit)
    {
        GameObject[] gameObjectsThatHited = new GameObject[laserRayCastPoints.Length];
        int[] countsRaysThatHitOnTheSameObject = new int[laserRayCastPoints.Length];


        //Find The object that get hit the most by the raycast into currentGameObject
        for (int i = 0; i < laserRayCastPoints.Length; i++)
        {
            if (rayCastThatHit[i].collider == null)
            {
                goto OuterLoop;
            }
            for (int j = 0; j <= i; j++)
            {
                if (gameObjectsThatHited[j] == null)
                {
                    gameObjectsThatHited[j] = rayCastThatHit[i].collider.gameObject;
                    countsRaysThatHitOnTheSameObject[j]++;
                    goto OuterLoop;
                }

                if (gameObjectsThatHited[j] == rayCastThatHit[i].collider.gameObject)
                {
                    countsRaysThatHitOnTheSameObject[j]++;
                    goto OuterLoop;
                }

            }
        OuterLoop:
            continue;

        }

        int index = 0;
        float count = 0f;
        for (int i = 0; i < countsRaysThatHitOnTheSameObject.Length; i++)
        {
            if (count < countsRaysThatHitOnTheSameObject[i])
            {
                count = countsRaysThatHitOnTheSameObject[i];
                index = i;
            }

        }

        GameObject currentGameObject = gameObjectsThatHited[index];



        float tempDistance = 0;
        for (int i = 0; i < laserRayCastPoints.Length; i++)
        {
            if (rayCastThatHit[i].collider == null)
            {
                continue;
            }


            if (currentGameObject != rayCastThatHit[i].collider.gameObject)
            {
                continue;
            }

            
            if (tempDistance < rayCastThatHit[i].distance)
            {
                tempDistance = rayCastThatHit[i].distance;
            }

        }

        return tempDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("Hit");
        }
    }
}
