using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExplorationEvents
{
    public class ExploreEventTypes : Exploration
    {
        public void LinnearEventSequence()
        {
            if (isExploring)
            {
                //Start event from the array of the appropriate index.
                //These events are a class with enums that can be listed in order and quantity of desire in the inspector by others to grant grater flexibility in making events.
            }
        }

        private void SubEventSequence() 
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            //int subEventLength = GameManager.explorationEvents[GameManager.eventIndex].subEvent.Count;
            for (int subEventIndex = 0; subEventIndex < gameManager.explorationEvents[GameManager.eventIndex].subEvent.Count/*Mathf.Clamp(subEventLength, 0,subEventLength)*/; subEventIndex++) 
            {
                ExploreSubEvent subEvent = gameManager.explorationEvents[GameManager.eventIndex].subEvent[subEventIndex];
                switch (subEvent.eventType) 
                {
                    case (ExploreSubEvent.eventTypes.Text): PlayTextEvent(subEvent.textEvent.eventMessage);
                        break;
                    case (ExploreSubEvent.eventTypes.Item):
                        for (int itemIndex = 0; itemIndex < subEvent.itemEvent.loot.Length; itemIndex ++) 
                        {
                            PlayItemEvent(subEvent.itemEvent.addOrSubtract, subEvent.itemEvent.loot[itemIndex].lootItem, subEvent.itemEvent.maxAmmount);
                        } 
                        
                        break;
                    case (ExploreSubEvent.eventTypes.Damage): PlayDamageEvent(subEvent.damageEvent.damageRecieved);
                        break;
                    case (ExploreSubEvent.eventTypes.Combat): PlayCombatEvent(subEvent.combatEvent.damageDealt, subEvent.combatEvent.damageRecieved);
                        break;

                    default:
                        break;
                }
            }
        }
///////////////////////////////////////////////////////////////////////////////////////////////////////////// TextEvent    
//------------------------------------------------------------------------------------------
        [System.Serializable]
        public class TextEvent
        {
            [Tooltip("Displayed text message during text event.")]
            public string eventMessage;
        }
//------------------------------------------------------------------------------------------
        private void PlayTextEvent(string message)
        {
            TextLog.AddLog(message);
        }
///////////////////////////////////////////////////////////////////////////////////////////////////////////// ItemEvent    
//------------------------------------------------------------------------------------------        
        [System.Serializable]
        public class ItemEvent
        {
            [Tooltip("Displayed text message during item event.")]
            public string eventMessage;

            [Tooltip("True = Add items, False = Subtract items.")]
            public bool addOrSubtract;

            [Tooltip("Maximum possible ammount of given items dropped (For now it is the only ammount).")]
            public int maxAmmount;

            [Tooltip("Minimum possible ammount of given items dropped.")]
            public int minAmmount;

            [Tooltip("Items that can drop during item event.")]
            public Looting.LootItem[] loot;
        }
//------------------------------------------------------------------------------------------
        private void PlayItemEvent(/*True = add item, False = remove item*/bool isAdding, Item item, int quantity)
        {
            if (quantity == 0)
            {
                Debug.LogWarning("You added or subtracted 0 items. This does nothing: ItemEvent cancelled.");
                return;
            }
            else if (quantity < 0)
            {
                Debug.LogError("You added or subtracted a negataive value of items. This is not allowed: ItemEvent cancelled. If you are trying to subtract items, set bool parameter 'Add' to false and input a positive value.");
                return;
            }


            if (isAdding)
            {
                Inventory.AddItem(item, quantity);
            }
            else
            {
                Inventory.RemoveItem(item, quantity);
            }
        }

        private void PlayItemEvent(/*True = add item, False = remove item*/bool Add, Item item)
        {
            if (item == null)
            {
                return;
            }
            if (Add)
            {
                Inventory.AddItem(item);
            }
            else
            {
                Inventory.RemoveItem(item);
            }
        }
///////////////////////////////////////////////////////////////////////////////////////////////////////////// DamageEvent
//------------------------------------------------------------------------------------------
        [System.Serializable]
        public class DamageEvent
        {
            [Tooltip("Damage recieved during damage event.")]
            public int damageRecieved;
        }
//------------------------------------------------------------------------------------------
        private void PlayDamageEvent(float damage)
        {
            TakeDamage(damage);
            PlayTextEvent(gameObject.name + " took " + damage + " damage to their health.");
        }
///////////////////////////////////////////////////////////////////////////////////////////////////////////// CombatEvent
//------------------------------------------------------------------------------------------
        [System.Serializable]
        public class CombatEvent 
        {
            [Tooltip("Damage dealt during combat event.")]
            public int damageDealt;
            [Tooltip("Damage recieved during combat event.")]
            public int damageRecieved;
            [Tooltip("Displayed text message during combat event.")]
            public string combatEventMessage;
            [Tooltip("Items that can drop after combat event.")]

            public Looting.CombatLootItem[] combatLoot = new Looting.CombatLootItem[System.Enum.GetNames(typeof(ExploreSubEvent.enemyFactions)).Length];
        }
//------------------------------------------------------------------------------------------
        private void PlayCombatEvent(float damageDealt, float damageRecieved)
        {
            if (damageDealt == 0 && damageRecieved == 0)
            {
                Debug.LogWarning("No values input. This does nothing: CombatEvent cancelled.");
                return;
            }

            PlayTextEvent(gameObject.name + " engaged in combat against hostile scavengers and dealt" + damageDealt + "damage to scavengers.");
            PlayDamageEvent(Random.Range(1, damageRecieved));
        }
        private void PlayCombatEvent(float damageDealt, float damageRecieved, string message)
        {
            if (damageRecieved == 0 && damageRecieved == 0 && message == null || damageRecieved == 0 && damageRecieved == 0 && message == "")
            {
                Debug.LogWarning("No values input. This does nothing: CombatEvent cancelled.");
                return;
            }
        }
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void TakeDamage(float damage)
        {
            Character character = gameObject.GetComponent<Character>();

            if (!character)
            {
                return;
            }
            if (character.health <= 0)
            {
                return;
            }

            Mathf.Clamp(character.health -= damage, 0, character.maxHealth);
        }
    }   
    [System.Serializable]
    public class ExploreEvent
    {
        public string eventName;
        public List<ExploreSubEvent> subEvent;
    }

    [System.Serializable]
    public class ExploreSubEvent
    {
        public enum eventTypes { Text, Item, Damage, Combat };
        public eventTypes eventType;

        public enum enemyFactions { Scavengers, Mutated_dogs, Radioactive_lobsters };

        [Header("Event type variables")]
        public ExploreEventTypes.TextEvent textEvent;
        public ExploreEventTypes.ItemEvent itemEvent;
        public ExploreEventTypes.DamageEvent damageEvent;
        public ExploreEventTypes.CombatEvent combatEvent;
    }
}