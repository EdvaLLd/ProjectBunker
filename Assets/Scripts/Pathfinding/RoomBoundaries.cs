using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBoundaries
{
    public Vector3 pos;
    public Vector3 size;


    public bool isAccessable;

    public List<RoomBoundaries> horizontalConnections = new List<RoomBoundaries>();
    public List<LadderConnections> laddersInRoom = new List<LadderConnections>();

    //H�r kan man nog h�lla koll p� dimman och ta bort den
}
