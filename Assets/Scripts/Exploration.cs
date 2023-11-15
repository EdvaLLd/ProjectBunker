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
    public enum environments { Home, City, Factory, Forest };
    [SerializeField]
    private environments environment;

    [SerializeField]
    private float distanceToHome;

    public string locationName = "Unknown Location";

    public void SetLocation(environments location, float distance) 
    {
        /*switch(location)
        {
            case environments.City:

                break;

            default:            
                break;
        }*/

        environment = location;
        distanceToHome = distance;
    }

    public void SetLocation(environments location, float distance, string name)
    {
        environment = location;
        distanceToHome = distance;
        locationName = name;
    }
}
