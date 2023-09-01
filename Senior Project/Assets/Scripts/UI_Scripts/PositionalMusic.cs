using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PositionalMusic : MonoBehaviour
{
    [SerializeField] private AudioSource ownSource;
    [SerializeField] private float maxVolume, minDistance, maxDistance;
    private float volume;
    private PlayerPuppet activePuppet;
    [SerializeField] private BossEnemyController bossInstance;

    private void Start()
    {
        activePuppet = PlayerController.puppet;
        if (bossInstance == null) bossInstance = FindObjectOfType<BossEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.temperature.stat >= 100 || PlayerController.instance.temperature.stat <= -100 || !bossInstance.gameObject.activeInHierarchy)
        {
            volume = 0;
            ownSource.volume = volume;
        }
        else if (activePuppet != null)
        {
            float dist = Vector3.Distance(transform.position, activePuppet.transform.position);
            if (dist > maxDistance)
            {
                volume = 0;
            }
            else
            {
                if (dist <= minDistance)
                {
                    volume = maxVolume;
                }
                else
                {
                    volume = (1 - ((dist - minDistance) / (maxDistance - minDistance))) * maxVolume;
                }
            }
            ownSource.volume = volume;
        }
    }
}
