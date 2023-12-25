using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    [SerializeField]
    private bool loadOnStart = true;
    [SerializeField]
    private bool saveOnQuit = true;
    [SerializeField]
    private string customDirectory = null;
    [SerializeField]
    private bool initializeNewOnLoadNull = true;
    private GameData gameData;

    private List<IDataPersistance> dataPersistanceObjects;

    [Header("File store config."), SerializeField]
    private string fileName;
    private FileDataHandler dataHandler;

    public static DataPersistanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More than one DataPersistanceManagers in scene!");
        }
        Instance = this;
    }

    private void Start()
    {
        LoadGameData();
    }

    public void NewGame() 
    {
        gameData = new GameData();
    }

    public void SaveGame() 
    {
        // Pass data to other scripts to update it.
        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects) 
        {
            dataPersistanceObject.SaveData(ref gameData);
        }
        //print("Saved: " + gameData.clockHour + "h, " + gameData.clockMinute + "m.");

        // Save data to a file using data handler.
        dataHandler.Save(gameData);
    }

    public void LoadGame() 
    {
        // Load saved data from a file using data handler (create data handler script).
        gameData = dataHandler.Load();

        //if there is no data -> new game.
        if(gameData == null && initializeNewOnLoadNull)
        {
            Debug.Log("No data found, initialising new game.");
            NewGame();
        }

        // Push loaded data to all scripts that need it.
        foreach (IDataPersistance dataPersistanceObject in dataPersistanceObjects) 
        {
            dataPersistanceObject.LoadData(gameData);
        }
        //print("Loaded: " + gameData.clockHour + "h, " + gameData.clockMinute + "m.");
    }

    private void OnApplicationQuit() // Temporary solution for now, button later.
    {
        if (saveOnQuit) 
        {
            SaveGame();
        }
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects() 
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistanceObjects);
    }

    private void LoadGameData() 
    {
        if (customDirectory == "" || customDirectory == null)
        {
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            print(Application.persistentDataPath + "/" + fileName);
        }
        else
        {
            dataHandler = new FileDataHandler(customDirectory, fileName);
            print(customDirectory + "/" + fileName);
        }

        dataPersistanceObjects = FindAllDataPersistanceObjects();
        if (loadOnStart)
        {
            LoadGame();
        }
    }
}