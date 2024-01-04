using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GearScore
{
    public int armor;
    public int attack;
}

[Serializable]
public class Equipment : Item
{
    //h�r ska det v�l in skit sen antar jag
    public Equipment()
    {
        SortingType = SortingTypes.Gear;
    }
    public GearTypes gearType;
}

[Serializable]
public enum GearTypes
{
    chest,
    legs,
    boots,
    weapon
}
