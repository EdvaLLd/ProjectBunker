using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploration : MonoBehaviour
{
    protected static bool executedEvent = false;
    protected static GameObject attachedGameObject;

    //[SerializeField]
    private int timeDivisor = 100;

    [SerializeField]
    private Locations.Location.environments currentEnvironment = Locations.Location.environments.Home;

    private void Awake()
    {
        attachedGameObject = gameObject;
    }

    public void Explore()
    {
        StartCoroutine(ExploringProcess());
    }

    public IEnumerator ExploringProcess() 
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        int randomLocationIndex = Random.Range(0, GameManager.GetExplorableLocations().Length);
        Locations.Location randomLocation = GetRandomExplorableLocation(randomLocationIndex);
        int randomItemIndex = GetRandomLocationItemIndex(randomLocation);
        float lootRandom = Random.Range(0, 100);

        Locations.Location.environments exploreLocation = randomLocation.environment;
        float distance = randomLocation.distanceToHome;
        string locationName = randomLocation.locationName;

        string startMessage = gameObject.GetComponent<Character>().characterName + " went to explore " + locationName + " near the " + exploreLocation + ".";
        string endMessage = gameObject.GetComponent<Character>().characterName + " returned from their trip to " + locationName + " near the " + exploreLocation + ".";
        string lootMessage = gameObject.GetComponent<Character>().characterName + " brought some " + "loot" + " with them.";
        string noLootMessage = gameObject.GetComponent<Character>().characterName + " didn't find anything useful.";


        //Item lootedItems = null;

        float timeToWait = distance / timeDivisor;
        currentEnvironment = exploreLocation;

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //gameObject.GetComponent<MeshRenderer>().enabled = false;

        //print
        TextLog.AddLog(startMessage);
        //isExploring = true;
        ExplorationEvents.ExploreEventTypes mainEvents = new ExplorationEvents.ExploreEventTypes();/*.LinnearEventSequence();*/

        yield return new WaitForSeconds(timeToWait * 3/4);

        mainEvents.LinnearEventSequence();
        if (!executedEvent)
        {
            mainEvents.RandomSpecialEvent();
        }

        yield return new WaitForSeconds(timeToWait * 1/4);

        
        currentEnvironment = Locations.Location.environments.Home;
        /*if (gameObject.GetComponent<Character>().health <= 0) 
        {
            gameObject.SetActive(false);
        }*/
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        // gameObject.GetComponent<MeshRenderer>().enabled = true;

        //print
        TextLog.AddLog(endMessage);
        //isExploring = false;

        yield return new WaitForSeconds(6);

        //float randomLocationIndex = Random.Range(0, GameManager.GetExplorableLocations().Length);

        //if (randomItemIndex != -1) 
        //{
        //if (gameManager.mainExploreEvents.Length <= 0) 
        //{
        if (!gameManager.mainExploreEvents[GameManager.eventIndex].replaceDefaultExplore /*!executedEvent*/) // Please forgive me for the ammount of if nestles. - Alexander Krasnov.
        {
            //print("Length: " + randomLocation.locationLoot.Count + ", i: " + randomItemIndex + ", Drop probability: " + randomLocation.locationLoot[randomItemIndex].lootProbability + "%");
            //print("Drop probability: " + randomLocation.lootProbabilities[randomItemIndex] + "%");
            if (randomItemIndex == -1) 
            {
                TextLog.AddLog(noLootMessage);
            }
            else if (lootRandom <= 100 - randomLocation.locationLoot[randomItemIndex].lootProbability && lootRandom != 100 || randomLocation.locationLoot[randomItemIndex].lootItem == null)
            {
                TextLog.AddLog(noLootMessage);

                if (randomLocation.locationLoot[randomItemIndex].lootItem == null)
                {
                    Debug.LogWarning("Item is null at element of index: " + randomItemIndex + " for location: " + exploreLocation + " ( element " + randomLocationIndex + " ). Check list for null items and remove elements or add item in 'Loot Item' slot.");
                }
            }
            else //if(Random.Range(0, 100) > noLootProbability)
            {
                //print
                //GameManager gameManager = FindObjectOfType<GameManager>();

                int maxQuantity = randomLocation.locationLoot[randomItemIndex].maxLootQuantity;
                int minQuantity = randomLocation.locationLoot[randomItemIndex].minLootQuantity;
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
        //}
        //}
        if (executedEvent) {executedEvent = false; }
    }
    
    private Locations.Location GetRandomExplorableLocation(int locationIndex) 
    {
        return GameManager.GetExplorableLocations()[locationIndex];
    }

    private int GetRandomLocationItemIndex(Locations.Location location)
    {
        if (location.locationLoot.Count <= 0)
        {
            Debug.LogWarning("No items present in location loot list for location: " + location.environment + ".");
            return -1;
        }
        return Random.Range(0, Mathf.Clamp(location.locationLoot.Count, 0, location.locationLoot.Count-1));
    }

    private Item GetRandomLocationItem(Locations.Location location, int itemIndex)
    {
        Item output = location.locationLoot[itemIndex].lootItem;
        print(location.environment + " " + output.name + " obtained." );

        return output;
    }
}