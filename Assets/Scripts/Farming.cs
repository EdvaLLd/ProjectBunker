using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmingSlot
{
    public CraftingRecipe recipe;
    public float timer;
    public bool active;
    public GameObject window;
    //kanske något med antal gånger kvar innan den går sönder
}

public class Farming : InteractableItem
{
    [SerializeField]
    GameObject farmingGO, farmingSlots, farmingSeeds, addBoxGO;

    [SerializeField]
    GameObject slotPrefab, seedPrefab;

    [SerializeField]
    Item plantingBox;

    List<FarmingSlot> slots = new List<FarmingSlot>();

    Character characterAtStation = null;

    int currentlySelectedNumber = -1;

    bool isWorking = false;

    void IsWorkingChange()
    {
        //här kan man göra skit som ska ske när man börjar/slutar arbeta
        //om man börjar crafta, slutar crafta eller går därifrån
    }

    private void Update()
    {
        if(characterAtStation != null)
        {
            bool isWorking = false;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].active)
                {
                    isWorking = true;
                    slots[i].timer -= Time.deltaTime;
                    slots[i].window.transform.GetChild(0).GetComponent<Slider>().value = slots[i].timer;
                    if (slots[i].timer < 0)
                    {
                        CraftFinished(slots[i]);
                    }
                }
            }
            if(this.isWorking != isWorking)
            {
                this.isWorking = isWorking;
                IsWorkingChange();
            }
        }
    }

    void CraftFinished(FarmingSlot slot)
    {
        Inventory.AddItem(slot.recipe.itemCrafted, slot.recipe.itemAmount);
        if (Inventory.IsCraftable(slot.recipe))
        {
            //autoloopar crafts nu
            StartCraft(slot);
            OpenSeedUI(currentlySelectedNumber);
        }
        else
        {
            slot.active = false;
            EmptySlot(slot);
        }
    }

    public void OpenUI(Character character)
    {
        UIManager.SetWindowActive(farmingGO);
        HelperMethods.ClearChilds(farmingSlots.transform);
        characterAtStation = character;
        addBoxGO.GetComponent<Button>().interactable = Inventory.GetAmountOfItem(plantingBox) != 0;
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, farmingSlots.transform);
            slots[i].window = slot;
            if (slots[i].recipe != null)
            {
                UpdateSlot(slots[i]);
            }
            //av någon jävla anledning måste jag spara i i en variabel innan
            //jag skickar in den som parametervärde???
            int wtf = i;
            slot.GetComponent<Button>().onClick.AddListener(() => OpenSeedUI(wtf));
        }
        if(slots.Count != 0)
        {
            OpenSeedUI(0);
        }
    }

    void UpdateSlot(FarmingSlot farmingSlot)
    {
        if (farmingSlot.recipe != null)
        {
            farmingSlot.window.transform.GetChild(0).GetComponent<Slider>().maxValue = farmingSlot.recipe.craftingTime;

            farmingSlot.window.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = farmingSlot.recipe.itemCrafted.Icon;
            farmingSlot.window.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = farmingSlot.recipe.itemCrafted.name;
        }
        else 
        {
            farmingSlot.window.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = null;
            farmingSlot.window.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Empty";
        }
        farmingSlot.window.transform.GetChild(0).GetComponent<Slider>().value = farmingSlot.timer;
    }

    //när vi fixar så den uppdateras när man lootar något och fönstret är öppet
    //så är det bara att länka till inventoryupdate eventet och gå igenom alla knappar
    //och aktivera/inaktivera de som ska
    public void OpenSeedUI(int nr)
    {
        currentlySelectedNumber = nr;
        UIManager.SetWindowActive(farmingSeeds);
        HelperMethods.ClearChilds(farmingSeeds.transform);
        List<CraftingRecipe> seeds = Inventory.GetRecipesForMachine(item as CraftingMachine);
        foreach(CraftingRecipe s in seeds)
        {
            GameObject temp = Instantiate(seedPrefab, farmingSeeds.transform);
            temp.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = s.itemCrafted.Icon;
            temp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{s.itemCrafted.DisplayName} \n Time: {s.craftingTime}";
            int amount = Inventory.GetAmountOfItem(s.Ingredients[0].item);
            temp.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = amount.ToString();
            temp.GetComponent<Button>().onClick.AddListener(() => SeedClicked(nr, s));
            if (amount == 0)
            {
                temp.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void EmptySlot(FarmingSlot slot)
    {
        if(slot.active)
        {
            StopCraft(slot);
        }
        slot.recipe = null;
        slot.timer = 0;
        slot.active = false;
        UpdateSlot(slot);
    }

    public void CharacterLeftStation(Character character)
    {
        if (isWorking)
        {
            isWorking = false;
            IsWorkingChange();
        }
        characterAtStation = null;
    }

    public void SeedClicked(int nr, CraftingRecipe seed)
    {
        if (slots[nr].recipe != null) StopCraft(slots[nr]);
        slots[nr].recipe = seed;
        UpdateSlot(slots[nr]);
        StartCraft(slots[nr]);
        OpenSeedUI(nr);
    }

    //borde antagligen gå att kalla på från en "avbryt" knapp
    void StartCraft(FarmingSlot slot)
    {
        foreach(RecipeSlot i in slot.recipe.Ingredients)
        {
            Inventory.RemoveItem(i.item, i.amount);
        }
        slot.timer = slot.recipe.craftingTime;
        slot.active = true;
    }
    void StopCraft(FarmingSlot slot)
    {
        if(slot.recipe != null)
        {
            foreach (RecipeSlot i in slot.recipe.Ingredients)
            {
                Inventory.AddItem(i.item, i.amount);
                slot.active = false;
            }
        }
    }

    public void AddSlot()
    {
        if(Inventory.GetAmountOfItem(plantingBox) > 0)
        {
            Inventory.RemoveItem(plantingBox);
            slots.Add(new FarmingSlot());
            OpenUI(characterAtStation);
        }
        addBoxGO.GetComponent<Button>().interactable = Inventory.GetAmountOfItem(plantingBox) != 0;
    }
}
