using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk_Orb_Script : MonoBehaviour
{
    public TrailRenderer VFX;
    public GameObject SpawnPointA;
    public GameObject SpawnPointB;
    public GameObject HolderEmpty;
	public GameObject PillarVFX;
    [SerializeField] private GameObject target;
    private float nextTimeToFireA;
    private float nextTimeToFireB;
    [SerializeField] private float fireRateA = 0.5f;
    [SerializeField] private float fireRateB = 0.5f;
	[SerializeField] private float sliderAmount = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            target = null;
        }
    }
	
	private void FixedUpdate()
	{
        if (target != null && Time.time >= nextTimeToFireA)
        {
            nextTimeToFireA = Time.time + 1 / fireRateA;

            TrailRenderer trailA = Instantiate(VFX, new Vector3(SpawnPointA.transform.position.x + Random.Range(-0.5f, 0.5f), SpawnPointA.transform.position.y + Random.Range(-0.5f, 0.5f), SpawnPointA.transform.position.z + Random.Range(-0.5f, 0.5f)), SpawnPointA.transform.rotation);
            if (HolderEmpty != null)
                trailA.transform.parent = HolderEmpty.transform;

            StartCoroutine(SpawnTrail(trailA, target.gameObject));

            TrailRenderer trailB = Instantiate(VFX, new Vector3(SpawnPointB.transform.position.x + Random.Range(-0.5f, 0.5f), SpawnPointB.transform.position.y + Random.Range(-0.5f, 0.5f), SpawnPointB.transform.position.z + Random.Range(-0.5f, 0.5f)), SpawnPointB.transform.rotation);
            if (HolderEmpty != null)
                trailB.transform.parent = HolderEmpty.transform;

            StartCoroutine(SpawnTrail(trailB, target.gameObject));

            if (PillarVFX.transform.position.y >= -1.3)
            {
                sliderAmount = -0.02f;
                PillarVFX.transform.position = new Vector3(PillarVFX.transform.position.x, PillarVFX.transform.position.y + sliderAmount, PillarVFX.transform.position.z);
            }
            else
                sliderAmount = 0;
        }

        if (target == null && Time.time >= nextTimeToFireA)
        {
            nextTimeToFireA = Time.time + 1 / fireRateA;

            if (PillarVFX.transform.position.y < 1.3)
            {
                sliderAmount = 0.02f;
                PillarVFX.transform.position = new Vector3(PillarVFX.transform.position.x, PillarVFX.transform.position.y + sliderAmount, PillarVFX.transform.position.z);
            }
            else
                sliderAmount = 0;
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, GameObject player)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Slerp(startPosition, player.transform.position, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = player.transform.position;

        Destroy(trail.gameObject, trail.time);
    }
}
