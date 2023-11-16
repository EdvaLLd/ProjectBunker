using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploration : MonoBehaviour
{
    private bool isExploring = false;

    /*private enum environments { Home, City, Factory, Forest };*/
    [SerializeField]
    private Location.environments currentEnviornment;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetExplorationState();
    }

    private void SetExplorationState() 
    {
        if (currentEnviornment != Location.environments.Home)
        {
            isExploring = true;
        }
        else 
        {
            isExploring = false;
        }
    }
}

public class Location : MonoBehaviour 
{
    [SerializeField]
    private float maxDistance = 10000;
    [SerializeField]
    private float minDistance = 500;
    public enum environments { Lake, Home, City, Factory, Forest };
    //[SerializeField]
    public environments environment;

    //[SerializeField]
    public float distanceToHome;

    public string locationName = "Unknown Location";

    public float RandomDistance() 
    {
        return Random.Range(minDistance, maxDistance);
    }

    /*public Location SetLocation(environments location, float distance, string name) 
    {
        Location returnedLocation = new Location();
       
        returnedLocation.environment = location;
        returnedLocation.distanceToHome = distance;
        returnedLocation.locationName = name;

        return returnedLocation;
    }*/
}
