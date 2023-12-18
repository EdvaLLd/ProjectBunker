using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class ExplorationBase
{
    public class ExploreEventTypes : ExplorationEvents
    {
        public bool LinnearEventSequence(Character character)
        {

            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            float eventRandom = UnityEngine.Random.Range(0, 100);
            float probability = gameManager.mainExploreEvents[GameManager.eventIndex].eventProbability;

            if (eventRandom <= 100 - probability && eventRandom != 100)
            {
                return false;
            }

            print("started event named: " + gameManager.mainExploreEvents[GameManager.eventIndex].eventName);


            SubEventSequence(gameManager.mainExploreEvents[GameManager.eventIndex], character);

            if (GameManager.eventIndex < gameManager.mainExploreEvents.Length)
            {
                GameManager.eventIndex++;
            }

            print("ended event named: " + gameManager.mainExploreEvents[GameManager.eventIndex - 1].eventName);

            return true;
        }

        public bool RandomSpecialEvent(Character character)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            ExplorationBase.RandomExploreEvent randomEvent = gameManager.randomExploreEvents[UnityEngine.Random.Range(0, gameManager.randomExploreEvents.Length)];
            float eventRandom = UnityEngine.Random.Range(0, 100);
            float probability = randomEvent.eventProbability;

            if (!randomEvent.CanBeActivated())
            {
                return false;
            }

            if (eventRandom <= 100 - probability)
            {
                return false;
            }

            randomEvent.ActivateEvent();
            SubEventSequence(randomEvent, character);

            print("ended event named: " + randomEvent.eventName);

            return true;
        }

        public bool LimitedEvent(Character character)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            ExplorationBase.LimitedExploreEvent randomEvent = gameManager.limitedExploreEvents[UnityEngine.Random.Range(0, gameManager.limitedExploreEvents.Length)];
            float eventRandom = UnityEngine.Random.Range(0, 100);
            float probability = randomEvent.eventProbability;
            randomEvent = gameManager.limitedExploreEvents[0];

            if (!randomEvent.CanBeActivated())
            {
                return false;
            }

            if(randomEvent.bringsNewCharacter && UnitController.GetCharacters().Count > 10)
            {
                return false;
            }

            if (eventRandom <= 100 - probability)
            {
                return false;
            }

            randomEvent.ActivateEvent();
            SubEventSequence(randomEvent, character);

            print("ended event named: " + randomEvent.eventName);

            return true;
        }

        private void SubEventSequence(ExplorationBase.ExploreEvent exploreEvent, Character character)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
            //int subEventLength = GameManager.explorationEvents[GameManager.eventIndex].subEvent.Count;
            for (int subEventIndex = 0; subEventIndex < exploreEvent.subEvents.Count/*Mathf.Clamp(subEventLength, 0,subEventLength)*/; subEventIndex++)
            {
                ExplorationBase.ExploreSubEvent subEvent = exploreEvent.subEvents[subEventIndex];
                switch (subEvent.eventType)
                {
                    case (ExplorationBase.ExploreSubEvent.eventTypes.Text):
                        subEvent.textEvent.timer.CountDown();
                        PlayTextEvent(subEvent.textEvent.eventMessage, character);
                        break;
                    case (ExplorationBase.ExploreSubEvent.eventTypes.Item):
                        subEvent.itemEvent.timer.CountDown();
                        PlayItemEvent(false, subEvent);
                        break;

                    case (ExplorationBase.ExploreSubEvent.eventTypes.Damage):
                        subEvent.damageEvent.timer.CountDown();
                        PlayDamageEvent(subEvent, character);
                        break;
                    case (ExplorationBase.ExploreSubEvent.eventTypes.Combat):
                        subEvent.combatEvent.timer.CountDown();
                        PlayCombatEvent(subEvent, character);
                        break;
                    case (ExplorationBase.ExploreSubEvent.eventTypes.Recipe):
                        subEvent.recipeEvent.timer.CountDown();
                        PlayRecipeEvent(subEvent);
                        break;
                    case (ExplorationBase.ExploreSubEvent.eventTypes.Character):
                        subEvent.recipeEvent.timer.CountDown();
                        PlayCharacterEvent();
                        break;
                    case (ExplorationBase.ExploreSubEvent.eventTypes.Illness):
                        subEvent.illnessEvent.timer.CountDown();
                        PlayIllnessEvent(character);
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

        public void PlayTextEvent(string message, Character character)
        {
            if (message != "" || message != null)
            {
                //TextLog.AddLog(message);
                FindObjectOfType<SpecialExploringEvents>().ShowSpecialEvent(message, character.gameObject.name);
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
        public void PlayItemEvent(bool isRandom, ExplorationBase.ExploreSubEvent subEvent)
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
                    int quantity = UnityEngine.Random.Range(subEvent.itemEvent.loot[itemIndex].minLootQuantity, subEvent.itemEvent.loot[itemIndex].maxLootQuantity);

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

                    float dropRandom = UnityEngine.Random.Range(0, 100);
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
                int randomIndex = UnityEngine.Random.Range(0, subEvent.itemEvent.loot.Length - 1);
                Item item = subEvent.itemEvent.loot[randomIndex].lootItem;
                int quantity = UnityEngine.Random.Range(subEvent.itemEvent.loot[randomIndex].minLootQuantity, subEvent.itemEvent.loot[randomIndex].maxLootQuantity);

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
                    int quantity = UnityEngine.Random.Range(itemArray[itemIndex].minLootQuantity, itemArray[itemIndex].maxLootQuantity);

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


                    float dropRandom = UnityEngine.Random.Range(0, 100);
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
                int randomIndex = UnityEngine.Random.Range(0, itemArray.Length - 1);
                Item item = itemArray[randomIndex].lootItem;
                int quantity = UnityEngine.Random.Range(itemArray[randomIndex].minLootQuantity, itemArray[randomIndex].maxLootQuantity);

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
        public void PlayDamageEvent(ExplorationBase.ExploreSubEvent subEvent, Character target)
        {
            float damage = subEvent.damageEvent.damageRecieved;

            //TakeDamage(damage, target);
            TextLog.AddLog(target.name + " took " + damage + " damage to their health!");

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
            public ExplorationBase.ExploreSubEvent.enemyFactions enemyFaction;

            [Tooltip("Delay in ammount of time before event starts in selected units.")]
            public Timer timer = new Timer();
        }
        //------------------------------------------------------------------------------------------
        public void PlayCombatEvent(ExplorationBase.ExploreSubEvent subEvent, Character character)
        {
            int enemyStrength = 0;
            int enemyDefense = 0;
            int enemyHealth = 0;

            int charStrength = /*character.GetGearScore().attack +*/ 60;
            int charDefense = /*character.GetGearScore().armor + */10;

            switch (subEvent.combatEvent.enemyFaction)
            {
                case ExplorationBase.ExploreSubEvent.enemyFactions.GiantInsects:
                    enemyStrength = 20;
                    enemyDefense = 10;
                    enemyHealth = 40;
                    break;
                case ExplorationBase.ExploreSubEvent.enemyFactions.FeralDogs:
                    enemyStrength = 30;
                    enemyDefense = 20;
                    enemyHealth = 50;
                    break;
                case ExplorationBase.ExploreSubEvent.enemyFactions.TwoheadedFoxes:
                    enemyStrength = 40;
                    enemyDefense = 30;
                    enemyHealth = 60;
                    break;
                case ExplorationBase.ExploreSubEvent.enemyFactions.Scavengers:
                    enemyStrength = 70;
                    enemyDefense = 50;
                    enemyHealth = 100;
                    break;
                default:
                    Debug.Log("No enemy was chosen for combat.");
                    break;
            }
            
            int rounds = (int)Math.Ceiling(enemyHealth / (double)(charStrength - enemyDefense));
            character.TakeDamage(rounds * (enemyStrength - charDefense));
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
        public void PlayRecipeEvent(ExplorationBase.ExploreSubEvent subEvent)
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

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////// IllnessEvent
        //------------------------------------------------------------------------------------------

        [System.Serializable]
        public class IllnessEvent
        {
            public Timer timer = new Timer();
        }
        public void PlayIllnessEvent(Character character)
        {
            //character.AddDesease<Flu>();
        }


        [System.Serializable]
        public class SimpleLootEvent
        {
            public Item item;
            public int minAmount;
            public int maxAmount;
        }


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
        public List<ExplorationBase.ExploreSubEvent> subEvents;
        public float eventProbability = 10;
        public Timer timer;
    }

    [System.Serializable]
    public class RandomExploreEvent : ExploreEvent
    {
        public int cooldown;
        protected bool canBeActivated = true;
        protected int turnsSinceActivation = 0;

        public virtual void ActivateEvent()
        {
            canBeActivated = false;
        }

        public bool CanBeActivated()
        {
            return canBeActivated;
        }

        public virtual void IncreaseTurnsSinceActivated()
        {
            if (!canBeActivated)
            {
                turnsSinceActivation++;
            }
            if(turnsSinceActivation > cooldown)
            {
                canBeActivated = true;
                turnsSinceActivation = 0;
                Debug.Log(this.eventName + " är nu redo att köras igen!");
            }
        }
    }

    [System.Serializable]
    public class LimitedExploreEvent : RandomExploreEvent
    {
        public int maxTurns;
        public bool bringsNewCharacter = false;
        private int timesActivated;

        public override void ActivateEvent()
        {
            base.ActivateEvent();
            timesActivated++;
        }
        public override void IncreaseTurnsSinceActivated()
        {
            if(timesActivated < maxTurns || maxTurns == 0)
            {
                if (!canBeActivated)
                {
                    turnsSinceActivation++;
                }
                if (turnsSinceActivation > cooldown)
                {
                    canBeActivated = true;
                    turnsSinceActivation = 0;
                    Debug.Log(this.eventName + " är nu redo att köras igen!");
                }
            }
            else
            {
                TurnOffEvent();
            }
            
        }

        public void TurnOffEvent()
        {
            canBeActivated = false;
        }
    }

    [System.Serializable]
    public class StandardExploreEvent
    {
        public ExploreEventTypes.SimpleLootEvent[] loot;
    }

    [System.Serializable]
    public class ExploreSubEvent
    {
        public enum eventTypes { Text, Item, Damage, Combat, Recipe, Illness, Diary, Character };
        public eventTypes eventType;

        public enum enemyFactions { Scavengers, FeralDogs, Radioactive_lobsters, TwoheadedFoxes, GiantInsects };

        [Header("Event type variables")]
        public ExploreEventTypes.TextEvent textEvent;
        public ExploreEventTypes.ItemEvent itemEvent;
        public ExploreEventTypes.DamageEvent damageEvent;
        public ExploreEventTypes.CombatEvent combatEvent;
        public ExploreEventTypes.RecipeEvent recipeEvent;
        public ExploreEventTypes.CharacterEvent characterEvent;
        public ExploreEventTypes.IllnessEvent illnessEvent;
    }
}
