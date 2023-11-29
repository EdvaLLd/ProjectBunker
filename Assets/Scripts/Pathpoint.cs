using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just nu �r det nog b�st att ha en s�n h�r punkt i varje d�rr�ppning/�ndring i y-led
public class Pathpoint : MonoBehaviour
{
    public bool isLocked;

    public bool isLadder = false;

    public Pathpoint[] connections;

    [HideInInspector]
    public float distFromStart = 0;
    [HideInInspector]
    public float estDistToEnd = 0;
    [HideInInspector]
    public float total = 0;
    [HideInInspector]
    public Pathpoint discoveryPoint = null;

    int lockedFromAmountOfRooms = 0;

    public void AddLockedFrom()
    {
        lockedFromAmountOfRooms++;
        isLocked = true;
    }
    public void RemoveLockedFrom()
    {
        lockedFromAmountOfRooms--;
        if(lockedFromAmountOfRooms < 1)
        {
            isLocked = false;
        }
    }

    public void ResetPoint()
    {
        distFromStart = Mathf.Infinity;
        estDistToEnd = 0;
        total = 0;
        discoveryPoint = null;
    }
}
