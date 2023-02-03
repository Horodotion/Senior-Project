using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float laserLength;
    [SerializeField] bool isInfiniteSpeed;
    [SerializeField] float laserSpeed;
    [SerializeField] private float laserSize = 1;
    [SerializeField] GameObject[] laserRayCastPoints;
    [SerializeField] LayerMask layer;
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

        if (isInfiniteSpeed)
        {
            if (PointsThatHit(this.transform.localScale.z, layer) == 0)
            {
                this.transform.localScale = new Vector3(laserSize, laserSize, laserLength);
                return;
            }
            
        }

        if (laserLength > this.transform.localScale.z)
        {
            this.transform.localScale += new Vector3(0, 0, laserSpeed * Time.deltaTime);
        }
    }
        


    private int PointsThatHit(float distance, LayerMask thatLayer)
    {
        int temp = 0;
        Debug.Log(laserRayCastPoints.Length);
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? laserLength : distance, thatLayer);
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
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? laserLength : distance, thatLayer);
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
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? laserLength : distance, thatLayer);
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
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit, isInfiniteSpeed ? laserLength : distance, thatLayer);
            if (hit.collider == null)
            {
                return false;
            }
        }
        return true;
    }

    private float GetLongestDistanceFromPointsHit(float distance, LayerMask thatLayer)
    {
        float temp = 0;
        foreach (GameObject thisPoint in laserRayCastPoints)
        {
            Physics.Raycast(thisPoint.transform.position, thisPoint.transform.forward, out RaycastHit hit,isInfiniteSpeed? laserLength : distance, thatLayer);
            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.name);
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
