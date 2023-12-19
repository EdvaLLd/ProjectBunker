using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    private SkyboxController skyboxManager;

    private static Locations.Location[] explorableLocations = new Locations.Location[System.Enum.GetNames(typeof(Locations.Location.environments)).Length];

    [Header("Location")]
    //public Looting.LootItem looting = new Looting.LootItem();

    [Tooltip("0=Lake, 1=City, 2=Factory, 3=Forest"), NonReorderable]
    public Looting.LocationLootItems[] locationalLoot = new Looting.LocationLootItems[System.Enum.GetNames(typeof(Locations.Location.environments)).Length - 1];

    [Header("Diary")]
    public static int leftPageIndex = 0;
    
    
    public Diary gameDiary = new Diary();

    [Header("Events")]
    public static int eventIndex = 0;

    public ExplorationBase.StandardExploreEvent[] standardExploreEvents;
    public ExplorationBase.ExploreEvent[] mainExploreEvents;
    public ExplorationBase.RandomExploreEvent[] randomExploreEvents;
    public ExplorationBase.LimitedExploreEvent[] limitedExploreEvents;

    GameObject clock;




    [Header("Characters")]
    public string[] characterNames =
    {
        "Gonzalez",
        "John",
        "Emily",
        "Michael",
        "Sarah",
        "David",
        "Awe",
        "Jessica",
        "Matthew",
        "Amanda",
        "Christopher",
        "Elizabeth",
        "Emma",
        "Edvard",
        "Ella",
        "Cassandra",
        "Alexander",
        "Thomas",
        "Patricia",
    };

    private float hour = 6;
    private float minute;

    private void Awake()
    {
        instance = this;
        
        skyboxManager = GameObject.FindObjectOfType<SkyboxController>();

        explorableLocations = Locations.SetExplorableLocations();

        clock = GameObject.Find("DayTimeStamp");

        //this.gameDiary.UpdateDiaryGUI();
    }

    private void Update()
    {
        skyboxManager.DayAndNightCycle(skyboxManager.cycleRate);
        DigitalClock();
    }

    private void DigitalClock() 
    {
        SkyboxController skyboxManager = GameObject.FindObjectOfType<SkyboxController>();

        //SetMinute(skyboxManager);
        //SetHour(skyboxManager);
        
        minute += skyboxManager.cycleRate * 24/360 * 60 * Time.deltaTime;
        if (minute >= 60) { minute = 0; hour++; }
        
        //hour += skyboxManager.cycleRate / 60;
        if (hour >= 24) { hour = 0; }

        string displayTime = Mathf.FloorToInt(hour).ToString("00") + ":" + Mathf.FloorToInt(minute).ToString("00");

        clock.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = displayTime;
    }

    public static Locations.Location[] GetExplorableLocations() 
    {
        return explorableLocations;
    }
}