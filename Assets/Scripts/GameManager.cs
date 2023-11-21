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
    
    //[SerializeField, Header("Location")]
    private static Location[] explorableLocations = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];
    
    [/*SerializeField, */Header("Location"), ArrayElementTitle("money")]
    public LootItems[] locationalLoot = new LootItems[System.Enum.GetNames(typeof(Location.environments)).Length-1];

    //private float[] lootProbabilites = new float[System.Enum.GetNames(typeof(Location.environments)).Length-1];

    [SerializeField, Tooltip("The higher the index in LootItems list, the lower is the probability for it appearing. This value is the probability at index 0.")]
    private float noLootProbabilityDefault = .75f;
   
    private void Awake()
    {
        instance = this;
        
        skylight = GameObject.Find("Directional Light");
        SetExplorableLocations();
    }

    /*private void Start()
    {
        //SetLocationList();
    }*/

    private void Update()
    {
        //print("Gecko");
        DayAndNightCycle(cycleRate);
        
        //print(explorableLocations[index].locationName + " is " + explorableLocations[index].environment + " at a distance of " + explorableLocations[index].distanceToHome + " meters.");
        //print(System.Enum.GetNames(typeof(Location.environments)).GetValue(2));
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
            /*Location[] */replaceArray = new Location[System.Enum.GetNames(typeof(Location.environments)).Length - 1];


            Location.environments[] locationArray = (Location.environments[])System.Enum.GetValues(typeof(Location.environments));

            for (int arrayIndex = 0; arrayIndex < replaceArray.Length; arrayIndex++)
            {
                replaceArray[arrayIndex] = new Location();
                replaceArray[arrayIndex].environment = locationArray[arrayIndex+1];

                replaceArray[arrayIndex].distanceToHome = replaceArray[arrayIndex].RandomDistance();
                replaceArray[arrayIndex].locationName += " No_" + arrayIndex;
                replaceArray[arrayIndex].locationLoot = locationalLoot[arrayIndex].lootItems;
                
                for (int probabilityIndex = 0; probabilityIndex < locationalLoot.Length; probabilityIndex++) 
                {
                    //replaceArray[arrayIndex].lootProbabilites[probabilityIndex] = new float[locationalLoot[probabilityIndex].lootItems.Count];
                     replaceArray[arrayIndex].noLootProbabilities[probabilityIndex] = noLootProbabilityDefault / (probabilityIndex+1) * 100/*Mathf.Pow(noLootProbabilityMultiplyer, probabilityIndex + 1)*/;
                }

                //print(replaceArray[arrayIndex].environment);
            }

            
        }
        return explorableLocations = replaceArray;
    }
}

/*[System.Serializable]
public class ItemList 
{
    public List<Item> itemsList;
}*/

[System.Serializable]
public class LootItems
{
    public List<Item> lootItems;
}