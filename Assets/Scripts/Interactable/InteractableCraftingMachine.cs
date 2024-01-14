using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class InteractableCraftingMachine : InteractableItem, IDataPersistance
{
    CraftingRecipe currentRecipeBeingCrafted;
    int amountLeft = 1;
    int amountPayedFor = 0;
    float progress; //0-1
    Character characterOnStation;
    bool isCrafting = false;

    //CraftingWindow craftingWindow;

    private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private GameObject craftingVFX;

    GameObject progressSlider;

    bool initPos = false;


    private void Awake()
    {
        craftingVFX.gameObject.GetComponent<VisualEffect>().Stop();
        //craftingWindow = GameObject.FindGameObjectWithTag("CraftingWindow").GetComponent<CraftingWindow>();
        audioSource = gameObject.AddComponent<AudioSource>();

        if(audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f;
    }

    private void Start()
    {
        progressSlider = Instantiate(UIManager.progressSliderStatic, UIManager.canvas.transform);
        progressSlider.transform.SetAsFirstSibling();
        Vector3 pos = transform.position;
        //pos.x += transform.lossyScale.x / 2 * GetComponent<BoxCollider>().bounds.center.x;
        //pos.y += transform.lossyScale.y / 2 * GetComponent<BoxCollider>().bounds.center.y;

        pos.y += 0.1f;
        progressSlider.transform.position = Camera.main.WorldToScreenPoint(pos);
        progressSlider.SetActive(false);
    }

    public void InteractedWith(Character character)
    {
        UIManager.SetWindowActive(UIManager.craftingWindow);
        UIManager.craftingWindow.GetComponent<CraftingWindow>().InitCraftingWindow(character.item as CraftingMachine, this, character);
    }
    public void CraftItems(CraftingRecipe recipe, Character characterCrafting)
    {
        SetIsCrafting(true);
        if (characterOnStation != null && characterOnStation != characterCrafting)
        {
            CharacterLeftStation(characterOnStation);
        }
        characterOnStation = characterCrafting;

        //Animation stuff
        if (characterOnStation.gameObject.GetComponentInChildren<CharacterAnimation>() != null)
        {
            characterOnStation.gameObject.GetComponentInChildren<CharacterAnimation>().StartCrafting();
            audioSource.Play();
        }

        if (recipe != currentRecipeBeingCrafted)
        {
            currentRecipeBeingCrafted = recipe;
            RemoveItemsRequiredForCraft();
        }
        else
        {
            if(amountPayedFor < amountLeft)
            {
                RemoveItemsRequiredForCraft();
            }
            else if(amountPayedFor > amountLeft)
            {
                RefundItemsForCraft(amountPayedFor - amountLeft);
            }
        }

    }
    public void CharacterLeftStation(Character character)
    {
        if (character == characterOnStation && character != null)
        {
            //Animation stuff
            if (characterOnStation.gameObject.GetComponentInChildren<CharacterAnimation>() != null)
            {
                characterOnStation.gameObject.GetComponentInChildren<CharacterAnimation>().StopCrafting();
                audioSource.Pause();
            }
            character.ResetInteractedWith();
            characterOnStation = null;
            SetIsCrafting(false);
        }
    }

    public void SetAmount(int value)
    {
        amountLeft = value;
    }

    public int GetAmount()
    {
        return amountLeft;
    }

    public int GetPayedAmount()
    {
        return amountPayedFor;
    }

    public bool GetIsCrafting()
    {
        return isCrafting;
    }

    public void SetIsCrafting(bool value)
    {
        progressSlider.SetActive(value);
        if(!initPos)
        {
            Vector3 pos = Physics.ClosestPoint(transform.position + Vector3.up*10, GetComponent<BoxCollider>(), transform.position, Quaternion.identity);
            pos.y += .3f;
            progressSlider.transform.position = Camera.main.WorldToScreenPoint(pos);
        }
        isCrafting = value;
        if (value) {
            craftingVFX.gameObject.GetComponent<VisualEffect>().Play();
        }
        else {
            craftingVFX.gameObject.GetComponent<VisualEffect>().Stop();
        }
    }

    public CraftingRecipe GetRecipe()
    {
        return currentRecipeBeingCrafted;
    }

    public float GetProgress()
    {
        return progress;
    }

    public void CancelCraft()
    {
        progress = 0;
        //varför fuckar den här upp om den ligger under rad 82??????
        SetIsCrafting(false);
        RefundItemsForCraft();
        amountLeft = 1;
        amountPayedFor = 0;
        currentRecipeBeingCrafted=null;

    }

    bool RemoveItemsRequiredForCraft()
    {
        if (currentRecipeBeingCrafted != null)
        {
            if (Inventory.IsCraftable(currentRecipeBeingCrafted, amountLeft - amountPayedFor))
            {
                for (int i = 0; i < currentRecipeBeingCrafted.Ingredients.Count; i++)
                {
                    Inventory.RemoveItem(currentRecipeBeingCrafted.Ingredients[i].item, currentRecipeBeingCrafted.Ingredients[i].amount * (amountLeft-amountPayedFor));
                }
                amountPayedFor = amountLeft;
                return true;
            }
        }
        return false;
    }

    void RefundItemsForCraft()
    {
        RefundItemsForCraft(amountPayedFor);
    }

    void RefundItemsForCraft(int amountToRefund)
    {
        if (currentRecipeBeingCrafted != null)
        {
            for (int i = 0; i < currentRecipeBeingCrafted.Ingredients.Count; i++)
            {
                Inventory.AddItem(currentRecipeBeingCrafted.Ingredients[i].item, currentRecipeBeingCrafted.Ingredients[i].amount * amountToRefund);
            }
            amountPayedFor -= amountToRefund;
        }
    }

    private void Update()
    {
        if(isCrafting)
        {
            progress += Time.deltaTime / currentRecipeBeingCrafted.craftingTime * characterOnStation.workMultiplier;
            progressSlider.GetComponent<Slider>().value = progress;
            if(progress > 1)

            {
                amountLeft--;
                amountPayedFor--;
                progress = 0;
                Inventory.AddItem(currentRecipeBeingCrafted.itemCrafted, currentRecipeBeingCrafted.itemAmount);

                if (amountPayedFor < 1)
                {
                    SetIsCrafting(false);
                    UIManager.craftingWindow.GetComponent<CraftingWindow>().FinishedCrafting(this);
                    currentRecipeBeingCrafted = null;
                    amountLeft = 1;
                    //Kan även fixa med animationer och typ uigrejer här

                    //Animation stuff
                    if (characterOnStation.gameObject.GetComponentInChildren<CharacterAnimation>() != null)
                    {
                        characterOnStation.gameObject.GetComponentInChildren<CharacterAnimation>().StopCrafting();
                        audioSource.Pause();
                    }   
                }
                /*else
                {
                    if (!Inventory.IsCraftable(currentRecipeBeingCrafted))
                    {
                        //varna UI-mässigt att det sket sig
                        CharacterLeftStation(characterOnStation);
                        isCrafting = false;
                    }
                    else
                    {
                        RemoveItemsRequiredForCraft();
                    }
                }*/
            }
            UIManager.craftingWindow.GetComponent<CraftingWindow>().SetCraftingValues(this);
        }
    }

    public void LoadData(GameData data)
    {
        // Variable present on this = data.Corresponding variable present on GameData;
    }

    public void SaveData(ref GameData data)
    {
        // data.Corresponding variable present on GameData = Variable present on this;
    }
}
