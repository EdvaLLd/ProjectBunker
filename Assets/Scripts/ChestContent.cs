using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContentPair
{
    public Item item;
    public int amount;
}
public class ChestContent : MonoBehaviour
{
    [SerializeField]
    ContentPair[] inventory;

    [SerializeField]
    CraftingRecipe[] recipes;

    [SerializeField]
    bool containsDiaryEntry;

    [SerializeField]
    private AudioClip audioClip;
    private AudioSource audioSource;

    bool looted = false;
    float alfa = 1;

    float fadeTimer = 1;

    private void Awake()
    {
        audioSource =gameObject.AddComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.clip = audioClip;
        }
        audioSource.spatialBlend = 1.0f;
        audioSource.loop = false;

    }

    private void Update()
    {
        if(looted)
        {
            Color newColor = GetComponent<SpriteRenderer>().color;
            newColor.a = alfa;
            alfa -= Time.deltaTime / fadeTimer;
            if(alfa < 0)
            {
                Destroy(gameObject);
            }
            GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    public void CheckContent()
    {
        if (!looted)
        {
            Loot();
            looted = true;
        }
    }

    void Loot()
    {
        audioSource.Play();
        if (inventory.Length == 0)
        {
            TextLog.AddLog("Chest is empty");
        }
        else
        {
            foreach (ContentPair item in inventory)
            {
                Inventory.AddItem(item.item, item.amount);
            }
            for (int i = 0; i < recipes.Length; i++)
            {
                Inventory.AddRecipeToMachines(recipes[i]);
            }
        }
        if (containsDiaryEntry)
        {
            FindObjectOfType<DiaryManager>().AddDiaryEntry();
        }
    }
}
