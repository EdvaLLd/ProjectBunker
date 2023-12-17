using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float clockHour;
    public float clockMinute;

    public GameData() 
    {
        clockHour = 6;
        clockMinute = 0;
    }
}
