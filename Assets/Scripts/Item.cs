using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class Item : ItemBase
{
    public SortingTypes SortingType;
    public float lootProbabilityOverride = 0;

    /*private void OnEnable()
    {
        int nr = (FindObjectsOfTypeIncludingAssets(typeof(Item)) as Item[]).Length;
        string nrStringified = "01";
        if (nr < 10) nrStringified += "0";
        if (nr < 100) nrStringified += "0";
        ID = nrStringified + nr.ToString();
    }*/
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