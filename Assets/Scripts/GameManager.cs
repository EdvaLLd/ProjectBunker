using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    public enum difficulties { Easy, Normal, Hard };
    [SerializeField]
    private difficulties difficulty;

    [SerializeField, Header("Day/Night cycle")]
    private float DayNightValue = 0; //0.0f - 360.0f
    [SerializeField]
    private float cycleRate = 0.1f;

    private GameObject skylight;


    private static Location[] explorableLocations = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];

    [Header("Location")]
    public Looting.LootItem looting = new Looting.LootItem();

    [Tooltip("0=Lake, 1=City, 2=Factory, 3=Forest")]
    public Looting.LocationLootItems[] locationalLoot = new Looting.LocationLootItems[System.Enum.GetNames(typeof(Location.environments)).Length - 1];

    [Header("Events")]
    public static int eventIndex = 0;

    public ExplorationEvents.ExploreEvent[] explorationEvents;

    private void Awake()
    {
        instance = this;
        
        skylight = GameObject.FindGameObjectWithTag("Sun");
        SetExplorableLocations();
    }

    /*private void Start()
    {
    }*/

    private void Update()
    {
        DayAndNightCycle(cycleRate);
    }

    private void DayAndNightCycle(float rate)
    {
        float calculatedCycleRate = rate * Time.deltaTime;

        DayNightValue += calculatedCycleRate;

        skylight.transform.rotation = Quaternion.Euler(DayNightValue, -38, 0);

        if (skylight.transform.rotation.x == 30)
        {

        }

        if (DayNightValue >= 360)
        {
            DayNightValue = 0;
        }
    }

    public static Location[] GetExplorableLocations() 
    {
        return explorableLocations;
    }

    private Location[] SetExplorableLocations()
    {
        Location[] replaceArray =  new Location[System.Enum.GetNames(typeof(Location.environments)).Length];
        if (System.Enum.GetNames(typeof(Location.environments)).Length > 1)
        {
            /*Location[] */replaceArray = new Location[System.Enum.GetNames(typeof(Location.environments)).Length - 1]; // -1 top reduce length by the first element (home).


            Location.environments[] locationArray = (Location.environments[])System.Enum.GetValues(typeof(Location.environments));

            for (int arrayIndex = 0; arrayIndex < replaceArray.Length; arrayIndex++) // arrayIndex loops over the the Location class objects in the Location[] replaceArray.
            {
                replaceArray[arrayIndex] = new Location();
                replaceArray[arrayIndex].environment = locationArray[arrayIndex+1]; // +1 to not loop over first element (home) and begin with the second one instead.

                // Sets up variables within the class.
                replaceArray[arrayIndex].distanceToHome = replaceArray[arrayIndex].RandomDistance();
                replaceArray[arrayIndex].locationName += " No_" + arrayIndex;
                replaceArray[arrayIndex].locationLoot = locationalLoot[arrayIndex].lootItems;
                replaceArray[arrayIndex].lootProbabilities = new float[locationalLoot[arrayIndex].lootItems.Count];
                
                //Sets up the probablity for the loot to drop if the RNG selected this item.
                for (int itemIndex = 0; itemIndex < locationalLoot[arrayIndex].lootItems.Count; itemIndex++) // itemIndex loops over lootable items in each Location class objects in replaceArray.
                {
                    //float probabilityAtIndex = (looting.lootProbabilityDefault / (itemIndex + 1)); // +1 because you can't divide by zero (I mean you can but you wouldn't get a usable number).

                    if (replaceArray[arrayIndex].locationLoot[itemIndex].lootItem.lootProbabilityOverride > 0) 
                    {
                        replaceArray[arrayIndex].lootProbabilities[itemIndex] = replaceArray[arrayIndex].locationLoot[itemIndex].lootItem.lootProbabilityOverride; // Overrides default probablity with value within item.
                    }
                    else 
                    {
                        replaceArray[arrayIndex].lootProbabilities[itemIndex] = replaceArray[arrayIndex].locationLoot[itemIndex].lootProbability; // assigned probability in inspector list for each item.
                    }
                }
            }
        }
        return explorableLocations = replaceArray; // Outputs an array without the first element of the enum with the approriate length of quantity of enums - 1.
    }
}