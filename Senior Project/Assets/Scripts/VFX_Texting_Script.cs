using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//To utilize the VFX you must have UnityEngine.VFX
using UnityEngine.VFX;

public class VFX_Texting_Script : MonoBehaviour
{
    //public variables for projectile fire points
    public GameObject Icicle_firePoint;
    public GameObject Fireball_firepoint;

    //public variables for projectiles
    public GameObject Icicle;
    public GameObject Fireball;

    //public variables for the projectile's shooting effects
    public VisualEffect Icicle_blast;

    //public variables for visual effect
    public VisualEffect flamethrower;
    public VisualEffect shotgun;


    // Start is called before the first frame update
    void Start()
    {
        //Stop the flamethrower effect
        flamethrower.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //flamethrower.Play();
            //Icicle_blast.Play();
            //call the projectile spawn function using the denomenator of 1 to signify that is is an icicle
            //SpawnVFX (1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            shotgun.Play();
            //call the projectile spawn function using the denomenator of 2 to signify that is is a fireball
            //SpawnVFX(2);
        }

        if (Input.GetMouseButtonUp(0))
        {
            flamethrower.Stop();
        }

        //if (Input.GetKeyDown(KeyCode.E))
        //{
            //This is the command to play the flamethrower effect
            //flamethrower.Play();
        //}

        //if (Input.GetKeyUp(KeyCode.E))
        //{
            //This is the command to stop the flamethrower effect
            //flamethrower.Stop();
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
            //This is the command to play the shotgun effect
            //shotgun.Play();
        //}
    }

    private void SpawnVFX(int Projectile)
    {
        //Icicle signifier
        if (Projectile == 1)
        {
            //Check to see if the Icicle has a fire point
            if (Icicle_firePoint != null)
            {
                //make a raycast
                Physics.Raycast(
                        origin: Icicle_firePoint.transform.position,
                        direction: Icicle_firePoint.transform.forward,
                        hitInfo: out RaycastHit hit,
                        maxDistance: 200f);

                
                    //Instantiate a copy of the Icicle
                    GameObject Shot = Instantiate(Icicle, Icicle_firePoint.transform.position, Icicle_firePoint.transform.rotation);
                    //Start Coroutine to calculate projectile's travel
                    StartCoroutine(SpawnProjectile(Shot, hit));
                
            }
            else
            {
                Debug.Log("No Icicle Fire Point Found");
            }
            
        }

        if (Projectile == 2)
        {
            //Check to see if the Fireball has a fire point
            if (Fireball_firepoint != null)
            {
                //make a raycast
                Physics.Raycast(
                        origin: Fireball_firepoint.transform.position,
                        direction: Fireball_firepoint.transform.forward,
                        hitInfo: out RaycastHit hit,
                        maxDistance: 200f);


                //Instantiate a copy of the Fireball
                GameObject Shot = Instantiate(Fireball, Fireball_firepoint.transform.position, Fireball_firepoint.transform.rotation);
                //Start Coroutine to calculate projectile's travel
                StartCoroutine(SpawnProjectile(Shot, hit));

            }
            else
            {
                Debug.Log("No Fireball Fire Point Found");
            }

        }
    }

    private IEnumerator SpawnProjectile(GameObject trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime;

            yield return null;
        }
        trail.transform.position = hit.point;

        Destroy(trail.gameObject, Time.time);
    }

}
