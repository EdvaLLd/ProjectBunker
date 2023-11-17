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

    [SerializeField]
    private float noLootProbability = 33;

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
        GameManager gameManager = FindObjectOfType<GameManager>();
        int randomIndex = Random.Range(0, gameManager.GetExplorableLocations().Length);
       
        Location.environments exploreLocation = gameManager.GetExplorableLocations()[randomIndex].environment;
        float distance = gameManager.GetExplorableLocations()[randomIndex].distanceToHome;
        string locationName = gameManager.GetExplorableLocations()[randomIndex].locationName;

        string startMessage = gameObject.name + " went to explore " + locationName + " near the " + exploreLocation + ".";
        string endMessage = gameObject.name + " returned from their trip to " + locationName + " near the " + exploreLocation + ".";
        string lootMessage = gameObject.name + " brought some " + "loot" + " with them.";
        string noLootMessage = gameObject.name + " didn't find anything useful.";

        //Item lootedItems = null;

        float timeToWait = distance / timeDivisor;
        currentEnvironment = exploreLocation;

        print(startMessage);
        
        yield return new WaitForSeconds(timeToWait);

        currentEnvironment = Location.environments.Home;

        print(endMessage);

        yield return new WaitForSeconds(2);

        if (Random.Range(0, 100) <= noLootProbability)
        {
            print(noLootMessage);
        }
        else /*if(Random.Range(0, 100) > noLootProbability)*/
        {
            print(lootMessage);
        }
    }

    public Location.environments GetCurrentEnvironment() 
    {
        return currentEnvironment;
    }
}

public class Location : MonoBehaviour 
{
    [SerializeField]
    private float maxDistance = 1500;
    [SerializeField]
    private float minDistance = 500;
    public enum environments { Lake, Home, City, Factory, Forest };
    //[SerializeField]
    public environments environment;

    //[SerializeField]
    public float distanceToHome;

    public string locationName = "Unknown Location";

    public float RandomDistance() 
    {
        return Random.Range(minDistance, maxDistance);
    }

    /*public Location SetLocation(environments location, float distance, string name) 
    {
        Location returnedLocation = new Location();
       
        returnedLocation.environment = location;
        returnedLocation.distanceToHome = distance;
        returnedLocation.locationName = name;

        return returnedLocation;
    }*/
}
