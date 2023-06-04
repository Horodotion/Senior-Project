using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserVer2 : MonoBehaviour
{
    [HideInInspector] public float laserDamageFrequency;
    [HideInInspector] public float laserDamage;
    [HideInInspector] private float timer;
    [SerializeField] private float maxlaserLength;
    [SerializeField] private bool isInfiniteSpeed;
    [SerializeField] private float laserSpeed;
    [SerializeField] private float laserSize = 1;
    [SerializeField] private GameObject[] laserRayCastPoints;
    [SerializeField] private LayerMask layer;
    [SerializeField] private GameObject iceBeam;
    [SerializeField] private GameObject fireBeam;
    private float currentLaserLegth;
    private RaycastHit[] rayCastThatHit;
    private bool isHittingPlayer = false;
    private float beamRange;
    private Vector3 beamEndPoint;
    private void Awake()
    {
        laserSize = laserSize * transform.localScale.z;
        rayCastThatHit = new RaycastHit[laserRayCastPoints.Length];
    }
    void OnEnable()
    {
        //this.transform.localScale = new Vector3(laserSize, laserSize, .1f);
    }

    void Update()
    {
        if (isInfiniteSpeed)
        {
            if (PointsThatHit(this.transform.localScale.z, layer) > 0)
            {
                beamRange = GetLongestDistanceFromPointsHit(this.transform.localScale.z, layer);
                //this.transform.localScale = new Vector3(laserSize, laserSize, GetLongestDistanceFromPointsHit(this.transform.localScale.z, layer));
                //return;
            }

            if (PointsThatHit(this.transform.localScale.z, layer) == 0)
            {
                beamRange = maxlaserLength;
                //this.transform.localScale = new Vector3(laserSize, laserSize, maxlaserLength);
                //return;
            }

        }
        else
        {
            if (PointsThatHit(this.transform.localScale.z, layer) > 0)
            {
                float temp = GetLongestDistanceFromPointsHit(maxlaserLength, layer);
                if (temp < this.transform.localScale.z)
                {
                    beamRange = temp;
                    //this.transform.localScale = new Vector3(laserSize, laserSize, temp);
                }
                currentLaserLegth = temp;
            }

            if (PointsThatHit(this.transform.localScale.z, layer) == 0)
            {
                currentLaserLegth = maxlaserLength;
            }


            if (currentLaserLegth > this.transform.localScale.z)
            {
                beamRange += laserSpeed * Time.deltaTime;
                //this.transform.localScale += new Vector3(0, 0, laserSpeed * Time.deltaTime);
            }
        }

        beamEndPoint = transform.position + (transform.forward * beamRange);

        if (laserDamage < 0)
        {
            if (fireBeam.activeInHierarchy)
            {
                fireBeam.SetActive(false);
            }

            if (!iceBeam.activeInHierarchy)
            {
                iceBeam.SetActive(true);
            }
            iceBeam.GetComponent<BeamScript>().ChangeEndPosition(beamEndPoint);
        }
        else
        {
            if (!fireBeam.activeInHierarchy)
            {
                fireBeam.SetActive(true);
            }

            if (iceBeam.activeInHierarchy)
            {
                iceBeam.SetActive(false);
            }
            fireBeam.GetComponent<BeamScript>().ChangeEndPosition(beamEndPoint);
        }

        

        isHittingPlayer = false;
        for (int i = 0; i < rayCastThatHit.Length; i++)
        {
            if (rayCastThatHit[i].collider != null)
            {
                if (rayCastThatHit[i].collider.tag.Equals("Player"))
                {
                    isHittingPlayer = true;
                }
            }
        }

        if (isHittingPlayer)
        {
            if (timer <= 0)
            {
                PlayerController.puppet.ChangeTemperature(laserDamage);
                timer = laserDamageFrequency;
            }
            else
            {
                timer -= Time.deltaTime;
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
        
        //RaycastHit[] rayCastThatHit = new RaycastHit[laserRayCastPoints.Length];
        
        for(int i = 0; i < laserRayCastPoints.Length; i++)
        {
            Physics.Raycast(laserRayCastPoints[i].transform.position, laserRayCastPoints[i].transform.forward, out rayCastThatHit[i],isInfiniteSpeed? maxlaserLength : rayCastDistance, thatLayer);
            //rayCastThatHit[i] = hit;
            
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
