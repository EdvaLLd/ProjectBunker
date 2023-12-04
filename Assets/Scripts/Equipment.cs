using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Equipment")]
public class Equipment : Item
{
    //här ska det väl in skit sen antar jag
    static int nr = 1;
    public Equipment()
    {
        SortingType = SortingTypes.Gear;
        ID = "0300"+nr.ToString();
        nr++;
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
