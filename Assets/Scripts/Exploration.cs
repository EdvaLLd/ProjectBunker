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

                int maxQuantity = gameManager.looting.maxLootQuantity;
                int minQuantity = gameManager.looting.minLootQuantity;
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
        Item output = location.locationLoot[/*Random.Range(0, location.locationLoot.Count)*/itemIndex];
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

    public List<Item> locationLoot;

    public float[] lootProbabilities;
    public float distanceToHome;

    public string locationName = "Unknown Location";

    public float RandomDistance() 
    {
        return Random.Range(minDistance, maxDistance);
    }
}

[System.Serializable]
public class LootItems
{
    public List<Item> lootItems;
}

[System.Serializable]
public class Looting
{
    public int maxLootQuantity;
    public int minLootQuantity;
    [Tooltip("The higher the index in LootItems list, the lower is the probability for it appearing. This value is the probability at index 0 in percent.")]
    public float lootProbabilityDefault = 75;
}

[System.Serializable]
public class ExplorationEventTypes : Exploration
{
    public void LinnearEventSequence(float exploreTime) 
    {
        if (isExploring) 
        {
            //Start event from the array of the appropriate index.
            //These events are a class with enums that can be listed in order and quantity of desire in the inspector by others to grant grater flexibility in making events.
        }
    }

    private void TextEvent(string message) 
    {
        TextLog.AddLog(message);
    }

    private void ItemEvent(/*True = add item, False = remove item*/bool Add, Item item, int quantity) 
    {
        if (quantity == 0)
        {
            Debug.LogWarning("You added or subtracted 0 items. This does nothing: ItemEvent cancelled.");
            return;
        }
        else if (quantity < 0) 
        {
            Debug.LogError("You added or subtracted a negataive value of items. This is not allowed: ItemEvent cancelled. If you are trying to subtract items, set bool parameter 'Add' to false and input a positive value.");
            return;
        }
        

        if (Add)
        {
            Inventory.AddItem(item, quantity);
        }
        else 
        {
            Inventory.RemoveItem(item, quantity);
        }
    }

    private void ItemEvent(/*True = add item, False = remove item*/bool Add, Item item)
    {
        if (item == null) 
        {
            return;
        }
        if (Add)
        {
            Inventory.AddItem(item);
        }
        else
        {
            Inventory.RemoveItem(item);
        }
    }

    private void DamageEvent(float damage) 
    {
        TakeDamage(damage);
        TextEvent(gameObject.name + " took " + damage + " damage to their health.");
    }

    private void CombatEvent(float damageDealt, float damageRecieved)
    {
        if (damageRecieved == 0 && damageRecieved == 0)
        {
            Debug.LogWarning("No values input. This does nothing: CombatEvent cancelled.");
            return;
        }

        TextEvent(gameObject.name + " engaged in combat against hostile scavengers");
        DamageEvent(Random.Range(0,30));
    }
    private void CombatEvent(float damageDealt, float damageRecieved, string message)
    {
        if (damageRecieved == 0 && damageRecieved == 0 && message == null || damageRecieved == 0 && damageRecieved == 0 && message == "") 
        {
            Debug.LogWarning("No values input. This does nothing: CombatEvent cancelled.");
            return;
        }


    }

    private void TakeDamage(float damage) 
    {
        Character character = gameObject.GetComponent<Character>();

        if (!character) 
        {
            return;
        }
        if (character.health <= 0) 
        {
            return;
        }

        Mathf.Clamp(character.health -= damage, 0, character.maxHealth);
    }
}

[System.Serializable]
public class ExplorationEvent
{
    public string eventName;
    public List</*ExplorationSubEvent*/ExplorationSubEvent> subEvent;
}

[System.Serializable]
public class ExplorationSubEvent
{
    public enum eventTypes { Text, Item, Damage, Combat };
    public eventTypes eventType;

    public enum combatFactions { Scavengers, Mutated_dogs, Radioactive_lobsters };

    [Header("TextEvent")]
    [Tooltip("Displayed text message during text event.")]
    public string eventMessage;

    [Header("ItemEvent")]
    [Tooltip("Items that can drop during item event.")]
    public Item[] loot;

    [Header("Damage/Combat-Event")]
    [Tooltip("Damage recieved during damage event. Will only take effect during eventType: Damage, Combat")]
    public int damageRecieved;

    [Header("CombatEvent")]
    [Tooltip("Damage dealt during combat event.")]
    public int damageDealt;
    [Tooltip("Displayed text message during combat event.")]
    public string customCombatMessage;
    [Tooltip("Items that can drop after combat event.")]
    
    public CombatLootItems[] combatLoot = new CombatLootItems[System.Enum.GetNames(typeof(combatFactions)).Length];
}

[System.Serializable]
public class CombatLootItems
{
    public ExplorationSubEvent.combatFactions enemyFaction;
    public List<Item> combatLootItems;
}