using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float maxlaserLength;
    [SerializeField] bool isInfiniteSpeed;
    [SerializeField] float laserSpeed;
    [SerializeField] private float laserSize = 1;
    [SerializeField] GameObject[] laserRayCastPoints;
    [SerializeField] LayerMask layer;
    private GameObject laseredObject;
    private float currentLaserLegth;
    private void Awake()
    {
        //this.transform.localScale = new Vector3(laserSize, laserSize, 1f);
    }
    void OnEnable()
    {
        this.transform.localScale = new Vector3(laserSize, laserSize, .1f);
    }

    void Update()
    {
        /*
        Debug.Log(this.transform.localScale.z);
        if (PointsThatHit(this.transform.localScale.z + 5, layer) == laserRayCastPoints.Length)
        {
            if (PointsThatHit(this.transform.localScale.z, layer) == laserRayCastPoints.Length)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, GetLongestDistanceFromPointsHit(this.transform.localScale.z, layer));
                return;
            }
        } 
        else
        
        if (PointsThatHit(this.transform.localScale.z + 3, layer) > 1)
        {
            if (PointsThatHit(this.transform.localScale.z, layer) > 1)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, GetLongestDistanceFromPointsHit(this.transform.localScale.z, layer));
                return;
            }
        }
        else if(PointsThatHit(this.transform.localScale.z, layer) == 1)
        {
            this.transform.localScale = new Vector3(laserSize, laserSize, GetLongestDistanceFromPointsHit(this.transform.localScale.z, layer));
            return;
        }
        */
        

        if (isInfiniteSpeed)
        {
            if (PointsThatHit(this.transform.localScale.z, layer) > 0)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, GetLongestDistanceFromPointsHit(this.transform.localScale.z + 1, layer));
                return;
            }

            if (PointsThatHit(this.transform.localScale.z, layer) == 0)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, maxlaserLength);
                return;
            }
            
        }


        if (PointsThatHit(this.transform.localScale.z, layer) > 0)
        {
            float temp = GetLongestDistanceFromPointsHit(maxlaserLength, layer) ;
            this.transform.localScale = new Vector3(laserSize, laserSize, temp);
            currentLaserLegth = temp;
        }

        if (PointsThatHit(this.transform.localScale.z, layer) == 0)
        {
            currentLaserLegth =  maxlaserLength;
        }


        if (currentLaserLegth > this.transform.localScale.z)
        {
            this.transform.localScale += new Vector3(0, 0, laserSpeed * Time.deltaTime);
        }
    }
        

    private GameObject IsChangingLaseredObjectNeeded(GameObject thatGameObject)
    {
        if (true)
        {

        }
        return null;
    }


    private int PointsThatHit(float rayCastDistance, LayerMask thatLayer)
    {
        int temp = 0;
        Debug.Log(laserRayCastPoints.Length);
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? maxlaserLength : rayCastDistance, thatLayer);
            //Debug.Log(hit.collider == null);
            if (hit.collider != null)
            {
                temp++;
            }
        }
        Debug.Log(temp);
        return temp;
    }

    private bool isASinglePointHit(float distance, LayerMask thatLayer)
    {
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? maxlaserLength : distance, thatLayer);
            if (hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }

    private bool isThreePointsHit(float distance, LayerMask thatLayer)
    {
        int temp = 0;
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? maxlaserLength : distance, thatLayer);
            if (hit.collider != null)
            {
                temp++;
            }
        }
        if (temp == 3)
        {
            return true;
        }
        return false;
    }

    private bool isAllPointsHit(float distance, LayerMask thatLayer)
    {
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? maxlaserLength : distance, thatLayer);
            if (hit.collider == null)
            {
                return false;
            }
        }
        return true;
    }

    private float GetLongestDistanceFromPointsHit(float rayCastDistance, LayerMask thatLayer)
    {
        float temp = 0;
        //GameObject[] gameObjectsThatHited = new GameObject[laserRayCastPoints.Length];
        //int[] countsRaysThatHitOnTheSameObject = new int[laserRayCastPoints.Length];
        //GameObject tempGameObject = null;
        for(int i = 0; i < laserRayCastPoints.Length; i++)
        {
            Physics.Raycast(laserRayCastPoints[i].transform.position, laserRayCastPoints[i].transform.forward, out RaycastHit hit,isInfiniteSpeed? maxlaserLength : rayCastDistance, thatLayer);
            if (hit.collider != null)
            {
                /*
                if (gameObjectsThatHited[0] == null)
                {
                    gameObjectsThatHited[0] = hit.collider.gameObject;
                    countsRaysThatHitOnTheSameObject[0]++;

                }

                if (hit.collider.gameObject != gameObjectsThatHited[i])
                {
                    countsRaysThatHitOnTheSameObject[i]++;
                }
                */
                
                if (hit.distance > temp)
                {
                    temp = hit.distance;
                }
            }
        }
        return temp;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("Hit");
        }
    }
}
