using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploration : MonoBehaviour
{
    //private bool isExploring = false;

    [SerializeField]
    private int timeDivisor = 100;

    [SerializeField]
    private Location.environments currentEnvironment = Location.environments.Home;

    /*[SerializeField]
    private float noLootProbability = 100;*/

    // Update is called once per frame
    /*void Update()
    {
        SetExplorationState();
    }*/

    /*private void SetExplorationState() 
    {
        if (currentEnviornment != Location.environments.Home)
        {
            isExploring = true;
           // return;
        }
        isExploring = false;
    }*/

    public IEnumerator ExploringProcess() 
    {
       // GameManager gameManager = FindObjectOfType<GameManager>();
        int randomIndex = Random.Range(0, GameManager.GetExplorableLocations().Length);
       
        Location.environments exploreLocation = GameManager.GetExplorableLocations()[randomIndex].environment;
        float distance = GameManager.GetExplorableLocations()[randomIndex].distanceToHome;
        string locationName = GameManager.GetExplorableLocations()[randomIndex].locationName;

        string startMessage = gameObject.name + " went to explore " + locationName + " near the " + exploreLocation + ".";
        string endMessage = gameObject.name + " returned from their trip to " + locationName + " near the " + exploreLocation + ".";
        string lootMessage = gameObject.name + " brought some " + "loot" + " with them.";
        string noLootMessage = gameObject.name + " didn't find anything useful.";


        //Item lootedItems = null;

        float timeToWait = distance / timeDivisor;
        currentEnvironment = exploreLocation;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        /*print*/
        TextLog.AddLog(startMessage);
        
        yield return new WaitForSeconds(timeToWait);

        currentEnvironment = Location.environments.Home;
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        /*print*/
        TextLog.AddLog(endMessage);

        yield return new WaitForSeconds(5);

        //float randomLocationIndex = Random.Range(0, GameManager.GetExplorableLocations().Length);
        Location randomLocation = GetRandomExplorableLocation();
        int randomItemIndex = GetRandomLocationItemIndex(randomLocation);

        if (Random.Range(0, 100) <= randomLocation.noLootProbabilities[randomItemIndex])
        {
            /*print*/
            TextLog.AddLog(noLootMessage);
        }
        else /*if(Random.Range(0, 100) > noLootProbability)*/
        {
            /*print*/
            TextLog.AddLog(lootMessage);
            Inventory.AddItem(randomLocation.locationLoot[randomItemIndex], Random.Range(1,4));
        }
    }

    public Location.environments GetCurrentEnvironment() 
    {
        return currentEnvironment;
    }

    public void Explore()
    {

        StartCoroutine(ExploringProcess());
    }

    private Location GetRandomExplorableLocation() 
    {
        return GameManager.GetExplorableLocations()[Random.Range(0, GameManager.GetExplorableLocations().Length)];
    }

    private int GetRandomLocationItemIndex(Location location)
    {
        return (int)location.noLootProbabilities[Random.Range(0,location.locationLoot.Count)];
    }
}

[System.Serializable]
public class Location : MonoBehaviour
{
    [SerializeField]
    private float maxDistance = 1500;
    [SerializeField]
    private float minDistance = 500;
    public enum environments { Home, Lake, City, Factory, Forest };
    public environments environment;

    public List<Item> locationLoot;

    public float[] noLootProbabilities;
    public float distanceToHome;

    public string locationName = "Unknown Location";

    public float RandomDistance() 
    {
        return Random.Range(minDistance, maxDistance);
    }
}
