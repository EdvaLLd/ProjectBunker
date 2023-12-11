using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Weapon")]
public class Weapon : Equipment
{
    [SerializeField]
    int attack;


    public int GetAttack()
    {
        return attack;
    }

    public Weapon()
    {
        gearType = GearTypes.weapon;
    }
}
