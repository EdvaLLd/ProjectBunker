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

    [SerializeField]
    private float DayNightValue = 0; //0.0f - 360.0f
    [SerializeField]
    private float cycleRate = 0.1f;

    private GameObject skylight;

    private Location[] explorableLocations = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];

    private void Awake()
    {
        instance = this;
        skylight = GameObject.Find("Directional Light");
    }

    private void Update()
    {
        //print("Gecko");
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

    private void SetLocationList(Location[] locations) 
    {
        for (int arrayIndex = 0; arrayIndex < locations.Length; arrayIndex++) 
        {

        }
    }
}
