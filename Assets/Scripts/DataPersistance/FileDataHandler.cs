using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirectoryPath = "";
    private string dataFileName = "";

    public FileDataHandler(string directoryPath, string fileName) 
    {
        dataDirectoryPath = directoryPath;
        dataFileName = fileName;
    }

    public GameData Load() 
    {
        // Path.Combine to account for different OS's using different path separators (selfish bastards, can never come together to unify stuff for some actual QoL improvement, Corpo slime).
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath)) 
        {
            try
            {
                // Load serialised data from file
                string readData = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        readData = reader.ReadToEnd();
                    }
                }

                // Deserialise and assign data to loadedData variable so it can later be returned at the end.
                loadedData = JsonUtility.FromJson<GameData>(readData);
            }
            catch (Exception e) 
            {
                Debug.LogError("Error when trying to load data from: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data) 
    {
        // Path.Combine to account for different OS's using different path separators (selfish bastards, can never come together to unify stuff for some actual QoL improvement, Corpo slime).
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);

        try
        {
            // creating dir. if it doesn't exist yet.
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialising saved C# data object into Json file
            string storeData = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) 
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(storeData);
                }
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error when trying to save data to: " + fullPath + "\n" + e);
        }
    }
}
