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

    /*[SerializeField, Header("Day/Night cycle")]
     private float DayNightValue = 0; //0.0f - 360.0f
     [SerializeField]
     private float cycleRate = 0.1f;
     [SerializeField]
     //private float angle = 0;
     private float moonSunOppositionAngle = 0;*/

    private SkyboxController skyboxManager;
    private GameObject sun;
    private GameObject moon;

    private static Location[] explorableLocations = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];

    [Header("Location")]
    public Looting looting = new Looting();
    
    [Tooltip("0=Lake, 1=City, 2=Factory, 3=Forest")]
    public LootItems[] locationalLoot = new LootItems[System.Enum.GetNames(typeof(Location.environments)).Length-1];
   
    private void Awake()
    {
        instance = this;
        
        skyboxManager = GameObject.FindObjectOfType<SkyboxController>();
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
        SetExplorableLocations();
    }

    /*private void Start()
    {
    }*/

    private void Update()
    {
        DayAndNightCycle(skyboxManager.cycleRate/*cycleRate, moonSunOppositionAngle*/);
        //moonSunOppositionAngle = moonSunOppositionAngle;
    }

    private void DayAndNightCycle(float rate/*, float moonAngle*/)
    {
        float calculatedCycleRate = rate * Time.deltaTime;

        skyboxManager.dayNightValue += calculatedCycleRate;

        sun.transform.rotation = Quaternion.Euler(skyboxManager.dayNightValue, -38, 0);
        moon.transform.rotation = Quaternion.Euler(skyboxManager.dayNightValue + 180 /*+ moonAngle*/, -38, 0);

       /* if (sun.transform.rotation.x == 30)
        {

        }*/

        if (skyboxManager.dayNightValue >= 360)
        {
            skyboxManager.dayNightValue = 0;
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
            /*Location[] */replaceArray = new Location[System.Enum.GetNames(typeof(Location.environments)).Length - 1];


            Location.environments[] locationArray = (Location.environments[])System.Enum.GetValues(typeof(Location.environments));

            for (int arrayIndex = 0; arrayIndex < replaceArray.Length; arrayIndex++) // arrayIndex loops over the the Location class objects in the Location[] replaceArray.
            {
                replaceArray[arrayIndex] = new Location();
                replaceArray[arrayIndex].environment = locationArray[arrayIndex+1];

                replaceArray[arrayIndex].distanceToHome = replaceArray[arrayIndex].RandomDistance();
                replaceArray[arrayIndex].locationName += " No_" + arrayIndex;
                replaceArray[arrayIndex].locationLoot = locationalLoot[arrayIndex].lootItems;
                replaceArray[arrayIndex].lootProbabilities = new float[locationalLoot[arrayIndex].lootItems.Count];
                
                for (int itemIndex = 0; itemIndex < locationalLoot[arrayIndex].lootItems.Count; itemIndex++) // itemIndex loops over lootable items in each Location class objects in replaceArray.
                {
                    float probabilityAtIndex = (looting.lootProbabilityDefault / (itemIndex + 1));

                    if (replaceArray[arrayIndex].locationLoot[itemIndex].lootProbabilityOverride > 0)
                    {
                        replaceArray[arrayIndex].lootProbabilities[itemIndex] = replaceArray[arrayIndex].locationLoot[itemIndex].lootProbabilityOverride;
                    }
                    else 
                    {
                        replaceArray[arrayIndex].lootProbabilities[itemIndex] = probabilityAtIndex;
                    }
                }
            }
        }
        return explorableLocations = replaceArray;
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

/*[System.Serializable]
public static class LightTest 
{
    public static float moonDirection;
    public static float viewDirection;
}*/