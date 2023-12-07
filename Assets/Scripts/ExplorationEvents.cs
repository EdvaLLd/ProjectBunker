using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExplorationEvents : Exploration
{

    public class ExploreEventTypes : ExplorationEvents
    {
        public void LinnearEventSequence()
        {
            executedEvent = true;
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            
            float eventRandom = Random.Range(0, 100);
            float probability = gameManager.mainExploreEvents[GameManager.eventIndex].eventProbability;

            if (eventRandom <= 100 - probability && eventRandom != 100) 
            {
                print("cancel");
                return;
            }

            print("started event named: " + gameManager.mainExploreEvents[GameManager.eventIndex].eventName);

            gameManager.mainExploreEvents[GameManager.eventIndex].timer.CountDown();

            SubEventSequence();

            if (GameManager.eventIndex < gameManager.mainExploreEvents.Length)
            {
                GameManager.eventIndex++;
            }
            
            print("ended event named: " + gameManager.mainExploreEvents[GameManager.eventIndex - 1].eventName);
        }

        //public void RandomSpecialEvent()
        //{
        //    Debug.Log("Yay, I am in the right place!");
        //    GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        //    Debug.Log(gameManager.randomExploreEvents[0].eventName);
        //}

        private void SubEventSequence()
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            //int subEventLength = GameManager.explorationEvents[GameManager.eventIndex].subEvent.Count;
            for (int subEventIndex = 0; subEventIndex < gameManager.mainExploreEvents[GameManager.eventIndex].subEvent.Count/*Mathf.Clamp(subEventLength, 0,subEventLength)*/; subEventIndex++)
            {
                ExploreSubEvent subEvent = gameManager.mainExploreEvents[GameManager.eventIndex].subEvent[subEventIndex];
                switch (subEvent.eventType)
                {
                    case (ExploreSubEvent.eventTypes.Text):
                        subEvent.textEvent.timer.CountDown();
                        PlayTextEvent(subEvent.textEvent.eventMessage);
                        break;
                    case (ExploreSubEvent.eventTypes.Item):
                            subEvent.itemEvent.timer.CountDown();
                            PlayItemEvent(false, subEvent);
                        break;

                    case (ExploreSubEvent.eventTypes.Damage):
                        subEvent.damageEvent.timer.CountDown();
                        PlayDamageEvent(subEvent, attachedGameObject.GetComponent<Character>());
                        break;
                    case (ExploreSubEvent.eventTypes.Combat):
                        subEvent.combatEvent.timer.CountDown();
                        PlayCombatEvent(subEvent, attachedGameObject.GetComponent<Character>());
                        break;
                    case (ExploreSubEvent.eventTypes.Recipe):
                        subEvent.recipeEvent.timer.CountDown();
                        PlayRecipeEvent(subEvent);
                        break;
                    case (ExploreSubEvent.eventTypes.Character):
                        subEvent.recipeEvent.timer.CountDown();
                        PlayCharacterEvent();
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

            [Tooltip("Delay in ammount of time before event starts in selected units.")]
            public Timer timer = new Timer();
        }
        //------------------------------------------------------------------------------------------

        public void PlayTextEvent(string message)
        {
            if (message != "" || message != null)
            {
                //TextLog.AddLog(message);
                FindObjectOfType<SpecialExploringEvents>().ShowSpecialEvent(message);
            }
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

            [Tooltip("Toggle whether to select one random item from list or give the entire list of items.")]
            public bool randomLoot;

            [Tooltip("Delay in ammount of time before event starts in selected units.")]
            public Timer timer = new Timer();

            [Tooltip("Items that can drop during item event.")]
            public Looting.LootItem[] loot;

        }
        //------------------------------------------------------------------------------------------
        public void PlayItemEvent(bool isRandom, ExploreSubEvent subEvent)
        {
            //GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            bool isAdding = subEvent.itemEvent.addOrSubtract;

            //if (subEvent.itemEvent.eventMessage != "" || subEvent.itemEvent.eventMessage != null) 
            //{
            //    PlayTextEvent(subEvent.itemEvent.eventMessage);
            //}

            if (!isRandom)
            {
                for (int itemIndex = 0; itemIndex < subEvent.itemEvent.loot.Length; itemIndex++)
                {
                    Item item = subEvent.itemEvent.loot[itemIndex].lootItem;
                    int quantity = Random.Range(subEvent.itemEvent.loot[itemIndex].minLootQuantity, subEvent.itemEvent.loot[itemIndex].maxLootQuantity);

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

                    float dropRandom = Random.Range(0, 100);
                    float probability = subEvent.itemEvent.loot[itemIndex].lootProbability;

                    if (item != null && dropRandom <= probability)
                    {
                        if (isAdding)
                        {
                            Inventory.AddItem(item, quantity);
                        }
                        else
                        {
                            Inventory.RemoveItem(item, quantity);
                        }
                    }

                }
            }
            else 
            {
                int randomIndex = Random.Range(0, subEvent.itemEvent.loot.Length-1);
                Item item = subEvent.itemEvent.loot[randomIndex].lootItem;
                int quantity = Random.Range(subEvent.itemEvent.loot[randomIndex].minLootQuantity, subEvent.itemEvent.loot[randomIndex].maxLootQuantity);

                if (item != null)
                {
                    if (isAdding)
                    {
                        Inventory.AddItem(item, quantity);
                    }
                    else
                    {
                        Inventory.RemoveItem(item, quantity);
                    }
                }
            }
        }

        public void PlayLootItemLoopEvent(bool isAdding, bool isRandom, Looting.LootItem[] itemArray)
        {
            //GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            if (!isRandom)
            {
                for (int itemIndex = 0; itemIndex < itemArray.Length; itemIndex++)
                {
                    Item item = itemArray[itemIndex].lootItem;
                    int quantity = Random.Range(itemArray[itemIndex].minLootQuantity, itemArray[itemIndex].maxLootQuantity);

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


                    float dropRandom = Random.Range(0, 100);
                    float probability = itemArray[itemIndex].lootProbability;

                    if (item != null && dropRandom <= probability)
                    {
                        if (isAdding)
                        {
                            Inventory.AddItem(item, quantity);
                        }
                        else
                        {
                            Inventory.RemoveItem(item, quantity);
                        }
                    }

                }
            }
            else 
            {
                int randomIndex = Random.Range(0, itemArray.Length - 1);
                Item item = itemArray[randomIndex].lootItem;
                int quantity = Random.Range(itemArray[randomIndex].minLootQuantity, itemArray[randomIndex].maxLootQuantity);

                if (item != null)
                {
                    if (isAdding)
                    {
                        Inventory.AddItem(item, quantity);
                    }
                    else
                    {
                        Inventory.RemoveItem(item, quantity);
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////// DamageEvent
        //------------------------------------------------------------------------------------------
        [System.Serializable]
        public class DamageEvent
        {
            [Tooltip("Damage recieved during damage event.")]
            public int damageRecieved;
            
            public Timer timer = new Timer();
        }
        //------------------------------------------------------------------------------------------
        public void PlayDamageEvent(ExploreSubEvent subEvent, Character target)
        {
            float damage = subEvent.damageEvent.damageRecieved;

            TakeDamage(damage, target);
            PlayTextEvent(target.name + " took " + damage + " damage to their health!");

            /*Timer localTimer = new Timer();
            localTimer.timeUnit = Timer.timeUnits.second;
            localTimer.ammount = 1;
            StartCoroutine(localTimer.CountDown());*/
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////// CombatEvent
        //------------------------------------------------------------------------------------------
        [System.Serializable]
        public class CombatEvent
        {
            [Tooltip("Hostile faction to be encountered during event.")]
            public ExploreSubEvent.enemyFactions enemyFaction;

            [Tooltip("Maximum possible damage dealt during combat event.")]
            public int maxDamageDealt;
            [Tooltip("Minimum possible damage dealt during combat event.")]
            public int minDamageDealt;

            [Tooltip("Maximum possible damage recieved during combat event.")]
            public int maxDamageRecieved;
            [Tooltip("Minimum possible damage recieved during combat event.")]
            public int minDamageRecieved;

            [Tooltip("Displayed text message during combat event.")]
            public string combatEventMessage;
            [Tooltip("Delay in ammount of time before event starts in selected units.")]
            public Timer timer = new Timer();
            [Tooltip("Items that can drop after combat event.")]
            public Looting./*Combat*/LootItem[] combatLoot/* = new Looting.CombatLootItem[System.Enum.GetNames(typeof(ExploreSubEvent.enemyFactions)).Length]*/;
        }
        //------------------------------------------------------------------------------------------
        public void PlayCombatEvent(ExploreSubEvent subEvent, Character character)
        {
            float randomRecievied = Random.Range(subEvent.combatEvent.minDamageRecieved, subEvent.combatEvent.maxDamageRecieved);
            float randomDealt = Random.Range(subEvent.combatEvent.minDamageDealt, subEvent.combatEvent.maxDamageDealt);
            string faction = System.Enum.GetName(typeof(ExploreSubEvent.enemyFactions), subEvent.combatEvent.enemyFaction);
            faction.Replace("_", " ");
            faction.ToLower();

            print(faction);

            //Make stuff happen here.
            if (subEvent.combatEvent.combatEventMessage != "" || subEvent.combatEvent.combatEventMessage != null)
            {
               TextLog.AddLog(subEvent.combatEvent.combatEventMessage);
            }

            //string faction = System.Enum.GetName(typeof(ExploreSubEvent.enemyFactions), subEvent.combatEvent.enemyFaction);
            //print(faction);                

            TextLog.AddLog(character.name + " engaged hostile " + faction + " in combat!");
            if (randomRecievied <= 0 && randomDealt <= 0)
            {
                TextLog.AddLog("Neither side sustained any casulties and fled.");
            }
            if (randomDealt > 0) 
            {
                TextLog.AddLog(character.name + " dealt " + randomDealt + " damage to the " + faction + ", weakening them.");
            }
            if (randomRecievied > 0)
            {
                TakeDamage(randomRecievied, character);
                if (character.health <= 0)
                {
                    TextLog.AddLog(character.name + " was slained in battle by the " + faction + ".");
                    //PlayLootItemLoopEvent(false, true, subEvent.combatEvent.combatLoot);
                }
                else 
                {
                    TextLog.AddLog("Enemy " + faction + " cowardly fled from battle.");
                    TextLog.AddLog(character.name + " took " + randomRecievied + " damage from the enemy " + faction + " but lives to fight another day.");
                    PlayLootItemLoopEvent(true, true, subEvent.combatEvent.combatLoot);
                }
                
            }

            //print("Combat");
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////// RecipeEvent
        //------------------------------------------------------------------------------------------
        [System.Serializable]
        public class RecipeEvent
        {
            [Tooltip("Recipe to learn")]
            public CraftingRecipe recipe;

            public Timer timer = new Timer();
        }
        //------------------------------------------------------------------------------------------
        public void PlayRecipeEvent(ExploreSubEvent subEvent)
        {
            Inventory.AddRecipeToMachines(subEvent.recipeEvent.recipe);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////// CharacterEvent
        //------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------
        [System.Serializable]
        public class CharacterEvent
        {
            public Timer timer = new Timer();
        }
        public void PlayCharacterEvent()
        {
            FindObjectOfType<SpecialExploringEvents>().ShowCharacterChoice();

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //------------------------------------------------------------------------------------------


    }
    private void TakeDamage(float damage, Character character)
    {
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

    [System.Serializable]
    public class ExploreEvent
    {
        public string eventName;
        public bool replaceDefaultExplore = true;
        public List<ExploreSubEvent> subEvent;
        public float eventProbability = 10;
        public Timer timer;
    }

    [System.Serializable]
    public class ExploreSubEvent
    {
        public enum eventTypes { Text, Item, Damage, Combat, Recipe, Sickness, Diary, Character };
        public eventTypes eventType;

        public enum enemyFactions { Scavengers, Mutated_dogs, Radioactive_lobsters, TwoheadedFoxes, GiantInsects };

        [Header("Event type variables")]
        public ExploreEventTypes.TextEvent textEvent;
        public ExploreEventTypes.ItemEvent itemEvent;
        public ExploreEventTypes.DamageEvent damageEvent;
        public ExploreEventTypes.CombatEvent combatEvent;
        public ExploreEventTypes.RecipeEvent recipeEvent;
        public ExploreEventTypes.CharacterEvent characterEvent;
    }
    
}