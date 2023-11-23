using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="Inventory System/Inventory Item")]
public class Item : ItemBase
{
    public SortingTypes SortingType;
}

[System.Serializable]
public enum SortingTypes
{
    All,
    Material,
    Food,
    Seed,
    Aid,
    Gear
};
