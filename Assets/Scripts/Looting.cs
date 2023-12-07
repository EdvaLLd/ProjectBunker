using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looting : MonoBehaviour
{
    [System.Serializable]
    public class LootItem /*Looting*/
    {
        public Item lootItem;
        public int maxLootQuantity;
        public int minLootQuantity;
        [Tooltip("Probability value for item to drop in percent.")]
        public float lootProbability = 75;
    }

    [System.Serializable]
    public class LocationLootItems //: LootItem
    {
        public List<LootItem> lootItems;
    }

    /*[System.Serializable]
    public class CombatLootItem : LootItem
    {
        public ExplorationEvents.ExploreSubEvent.enemyFactions enemyFaction;
    }*/
}
