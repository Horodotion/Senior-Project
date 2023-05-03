using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BossSpawnerController : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnData
    {
        public GameObject gameObjectToSpawn;
        public Transform gameObjectSPParent;
        public int numOfGameObjectSpawn;
    }

    [SerializeField] SpawnData[] spawnData;
    //[SerializeField] SpawnData turretsData
    //[SerializeField] SpawnData minesData;

    public void SpawningThings()
    {
        //SpawnItemBaseOnData(turretsData);
        //SpawnItemBaseOnData(minesData);
        for (int i = 0; i < spawnData.Length; i++)
        {
            SpawnItemBaseOnData(spawnData[0]);
        }
    }

    public void SpawnItemBaseOnData(SpawnData spawndata)
    {
        // This is to create a pot that can retieve a random number ticket that is inside the pot
        TicketPot pot = new TicketPot(spawndata.gameObjectSPParent.childCount);

        //The count variable is to make sure it wont spawn the object more than it needs to
        for (int i = 0, count = 0; i < spawndata.gameObjectSPParent.childCount && count < spawndata.numOfGameObjectSpawn; i++)
        {
            int spawnIndex = pot.PopRandomTicket();
            //Check if the spawn is empty
            if (spawndata.gameObjectSPParent.GetChild(spawnIndex).transform.childCount == 0)
            {
                //Spawn the Object and increase the count
                Instantiate(spawndata.gameObjectToSpawn, spawndata.gameObjectSPParent.GetChild(spawnIndex).transform);
                count++;
            }
        }
    }
}

// A class that store number tickets into a pot, and can retrieve a random number ticket that is inside of the pot.
public class TicketPot
{
    List<int> pot = new List<int>();
    public TicketPot(int size)
    {
        FillThePot(size);
    }
    public void FillThePot(int size)
    {
        
        for (int i = 0; i < size; i++)
        {
            pot.Add(i);
        }
        
    }
    //Get a random number ticket from the pot
    public int PopRandomTicket()
    {
        if (pot.Count > 0)
        {
            int index = Random.Range(0, pot.Count - 1);
            int temp = pot[index];
            pot.RemoveAt(index);
            return temp;
        }
        else
        {
            Debug.Log("Pot is empty");
            return -1;
        }
    }
    public int Count()
    {
        return pot.Count;
    }
}
