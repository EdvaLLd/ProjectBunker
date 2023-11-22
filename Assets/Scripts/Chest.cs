using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting System/Chest")]
public class Chest : ItemBase
{
    Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    /*float refillTimer = 30;
    float refillTimerMax = 30;
    float lastTimeChecked;*/

    bool looted = false;
    public void CheckContent()
    {
        /*float timeNow = Time.time;
        Debug.Log(refillTimer);
        if(lastTimeChecked == 0)
        {
            Debug.Log("first teime");
            GenerateContent();
        }
        else
        {
            refillTimer -= timeNow - lastTimeChecked;
            while(refillTimer < 0)
            {
                Debug.Log("generate!");
                refillTimer += refillTimerMax;
                GenerateContent();
            }
        }
        Debug.Log("loot?!");
        Loot();
        lastTimeChecked = timeNow;*/
        Debug.Log(looted);
        if(!looted)
        {
            GenerateContent();
            Loot();
            looted = true;
        }
    }

    void GenerateContent()
    {
        inventory.Add(Database.GetItemWithID("04001"), 10);
    }

    void Loot()
    {
        if (inventory.ContainsKey(Database.GetItemWithID("04001")))
        {
            Inventory.AddItem(Database.GetItemWithID("04001"), inventory[Database.GetItemWithID("04001")]);
            inventory.Clear();
        }
    }
}
