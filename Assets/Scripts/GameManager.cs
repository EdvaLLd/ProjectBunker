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

    /*[SerializeField]
    private int index;*/

    private GameObject skylight;

    private Location[] explorableLocations = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];

   
    private void Awake()
    {
        instance = this;
        
        skylight = GameObject.Find("Directional Light");
        SetLocationList();
    }

    private void Start()
    {
        //SetLocationList();
    }

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

    public Location[] GetExplorableLocations() 
    {
        return explorableLocations;
    }

    private void SetLocationList()
    {
        if (System.Enum.GetNames(typeof(Location.environments)).Length > 1)
        {
            Location.environments[] locationArray = (Location.environments[])System.Enum.GetValues(typeof(Location.environments));

            for (int arrayIndex = 1; arrayIndex < explorableLocations.Length; arrayIndex++)
            {
                explorableLocations[arrayIndex] = new Location();

                explorableLocations[arrayIndex].environment = locationArray[arrayIndex];
                explorableLocations[arrayIndex].distanceToHome = explorableLocations[arrayIndex].RandomDistance();
                explorableLocations[arrayIndex].locationName += " No_" + arrayIndex;

                //print(locationArray[arrayIndex]);

                /*if (explorableLocations[arrayIndex].environment == Location.environments.Home) 
                {
                    explorableLocations[arrayIndex].distanceToHome = 0;
                }*/
            }
        }
        else print("You have either no or no locations beyond Home defined in the Location.environment enum in Exploration.cs");
    }
}
