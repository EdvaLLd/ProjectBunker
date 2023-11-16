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

    [SerializeField]
    private float DayNightValue = 0; //0.0f - 360.0f
    [SerializeField]
    private float cycleRate = 0.1f;

    private bool doneOnce;

    private GameObject skylight;

    private Location[] explorableLocations = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];

    private void Awake()
    {
        instance = this;
        
        skylight = GameObject.Find("Directional Light");
        //PopulateLocationArray();
    }

    private void Start()
    {
        SetLocationList(explorableLocations);
    }

    private void Update()
    {
        //print("Gecko");
        DayAndNightCycle(cycleRate);
        //print(System.Enum.GetNames(typeof(Location.environments)).GetValue(2));

        print("hopefully: "+ explorableLocations[3].environment);
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

    /*private void PopulateLocationArray()
    {
        //if (!doneOnce) 
        //{
        for (int arrayIndex = 0; arrayIndex < explorableLocations.Length; arrayIndex++)
        {
            System.Enum.GetNames(typeof(Location.environments)).CopyTo(explorableLocations, arrayIndex);
        }
        //}
    }*/

    private void SetLocationList(Location[] locations)
    {
        //Location.environments[] locationArray = (Location.environments)System.Enum.GetValues(typeof(Location.environments));

        for (int arrayIndex = 0; arrayIndex < locations.Length; arrayIndex++) 
        {
            locations[arrayIndex] = new Location();

            Location.environments location = (Location.environments)System.Enum.GetNames(typeof(Location.environments)).GetValue(arrayIndex); //Casting retrieved object from enum at array index to match the type of Location.environment.

            locations[arrayIndex].SetLocation(location, locations[arrayIndex].RandomDistance(),"TempName "+arrayIndex);
        }
    }
}
