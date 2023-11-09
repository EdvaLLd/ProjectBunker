using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathpoint : MonoBehaviour
{
    public bool isLocked;
    public Pathpoint[] connections;

    [HideInInspector]
    public float distFromStart = 0;
    [HideInInspector]
    public float estDistToEnd = 0;
    [HideInInspector]
    public float total = 0;
    [HideInInspector]
    public Pathpoint discoveryPoint = null;

    public void ResetPoint()
    {
        distFromStart = Mathf.Infinity;
        estDistToEnd = 0;
        total = 0;
        discoveryPoint = null;
    }
}
