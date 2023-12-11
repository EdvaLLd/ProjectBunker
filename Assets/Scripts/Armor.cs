using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Armor")]
public class Armor : Equipment
{
    [SerializeField]
    int defence;

    public int GetDefence()
    {
        return defence;
    }
}
