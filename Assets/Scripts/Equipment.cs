using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearScore
{
    public int armor;
    public int attack;
}
public class Equipment : Item
{
    //här ska det väl in skit sen antar jag
    public Equipment()
    {
        SortingType = SortingTypes.Gear;
    }
    public GearTypes gearType;
}
public enum GearTypes
{
    chest,
    legs,
    boots,
    weapon
}
