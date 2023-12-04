using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory System/Food Item")]
public class Food : Item
{
    [SerializeField]
    float hungerRestoration;


    public float GetHungerRestoration()
    {
        return hungerRestoration;
    }
}
