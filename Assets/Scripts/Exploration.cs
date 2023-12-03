using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Exploration : MonoBehaviour
{
    protected bool isExploring = false;

    [SerializeField]
    private int timeDivisor = 100;

    [SerializeField]
    private Location.environments currentEnvironment = Location.environments.Home;

    public IEnumerator ExploringProcess() 
    {
       // GameManager gameManager = FindObjectOfType<GameManager>();
        int randomLocationIndex = Random.Range(0, GameManager.GetExplorableLocations().Length);
        Location randomLocation = GetRandomExplorableLocation(randomLocationIndex);
        int randomItemIndex = GetRandomLocationItemIndex(randomLocation);
        float noLootRandom = Random.Range(0, 100);

        Location.environments exploreLocation = randomLocation.environment;
        float distance = randomLocation.distanceToHome;
        string locationName = randomLocation.locationName;

        string startMessage = gameObject.name + " went to explore " + locationName + " near the " + exploreLocation + ".";
        string endMessage = gameObject.name + " returned from their trip to " + locationName + " near the " + exploreLocation + ".";
        string lootMessage = gameObject.name + " brought some " + "loot" + " with them.";
        string noLootMessage = gameObject.name + " didn't find anything useful.";


        //Item lootedItems = null;

        float timeToWait = distance / timeDivisor;
        currentEnvironment = exploreLocation;

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //gameObject.GetComponent<MeshRenderer>().enabled = false;

        /*print*/
        TextLog.AddLog(startMessage);
        isExploring = true;
        
        yield return new WaitForSeconds(timeToWait);

        currentEnvironment = Location.environments.Home;

        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        // gameObject.GetComponent<MeshRenderer>().enabled = true;

        /*print*/
        TextLog.AddLog(endMessage);
        isExploring = false;

        yield return new WaitForSeconds(6);

        //float randomLocationIndex = Random.Range(0, GameManager.GetExplorableLocations().Length);

        if (randomItemIndex != -1) 
        {
            print("Length: " + randomLocation.lootProbabilities.Length + ", i: " + randomItemIndex + ", Drop probability: " + randomLocation.lootProbabilities[randomItemIndex] + "%");
            //print("Drop probability: " + randomLocation.lootProbabilities[randomItemIndex] + "%");

            if (noLootRandom <= 100 - randomLocation.lootProbabilities[randomItemIndex] && noLootRandom < 100)
            {
                /*print*/
                TextLog.AddLog(noLootMessage);
            }
            else /*if(Random.Range(0, 100) > noLootProbability)*/
            {
                /*print*/
                GameManager gameManager = FindObjectOfType<GameManager>();

                int maxQuantity = /*gameManager.looting.maxLootQuantity;*/ randomLocation.locationLoot[randomItemIndex].maxLootQuantity;
                int minQuantity = /*gameManager.looting.minLootQuantity;*/ randomLocation.locationLoot[randomItemIndex].minLootQuantity;
                if ((maxQuantity <= 0 || minQuantity <= 0) || (maxQuantity <= 0 && minQuantity <= 0))
                {
                    TextLog.AddLog(lootMessage);
                    Inventory.AddItem(GetRandomLocationItem(randomLocation, randomItemIndex));
                }
                else 
                {
                    TextLog.AddLog(lootMessage);
                    Inventory.AddItem(GetRandomLocationItem(randomLocation, randomItemIndex), Random.Range(minQuantity, maxQuantity));
                }
            }
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

    private Location GetRandomExplorableLocation(int locationIndex) 
    {
        return GameManager.GetExplorableLocations()[locationIndex];
    }

    private int GetRandomLocationItemIndex(Location location)
    {
        if (location.lootProbabilities.Length <= 0)
        {
            Debug.LogWarning("No items present in location loot list probably for location: " + location.environment + ".");
            return -1;
        }
        else if (location.lootProbabilities.Length == 1) 
        {
            return 0;
        }
        return /*(int)location.lootProbabilities[Random.Range(0,location.locationLoot.Count-1)]*/Random.Range(0, location.locationLoot.Count);
    }

    private Item GetRandomLocationItem(/*int locationIndex, int itemIndex*/Location location, int itemIndex) 
    {
        //GameManager gameManager = FindObjectOfType<GameManager>();
        Item output = location.locationLoot[/*Random.Range(0, location.locationLoot.Count)*/itemIndex].lootItem;
        print(location.environment + " " + output.name + " obtained." );

        return output/*location.locationLoot[Random.Range(0, location.locationLoot.Count)]*/;
    }
}

[System.Serializable]
public class Location/* : MonoBehaviour*/
{
    [SerializeField]
    private float maxDistance = 1500;
    [SerializeField]
    private float minDistance = 500;
    public enum environments { Home, Lake, City, Factory, Forest };
    public environments environment;

    public List<Looting.LootItem> locationLoot;

    public float[] lootProbabilities;
    public float distanceToHome;

    public string locationName = "Unknown Location";

    public float RandomDistance() 
    {
        return Random.Range(minDistance, maxDistance);
    }
}
