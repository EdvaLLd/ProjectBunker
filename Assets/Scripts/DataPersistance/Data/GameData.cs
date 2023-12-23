using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{   
    #region Variables
    #region GameManager
    
    #region Time
    public float clockHour;
    public float clockMinute;
    public int dayCount;
    #endregion

    #region Diary
    public int diaryLeftPageIndex;
    public Diary diary;
    #endregion

    #region Event
    public int mainEventIndex;
    #endregion

    #region Character
    public CharacterList listOfCharacters;
    #endregion
    #endregion


    #region Skybox
    public float dayNightNumber;
    #endregion


    #region InteractableCraftingMachine
    #region CraftingTable
    public bool tableIsCrafting;
    public float craftingProgress;
    public CraftingRecipe currentTableRecepie;
    public Character tableUser;
    #endregion

    #region Stove
    public bool stoveIsBaking;
    public float bakingInProgress;
    public CraftingRecipe currentBakingRecepie;
    public Character stoveUser;
    #endregion

    #region Well
    public bool wellIsFetching;
    public float fetchingProgress;
    public CraftingRecipe currentWellRecepie;
    public Character wellUser;
    #endregion

    #region SewingMachine
    public bool sewingMachineIsSewing;
    public float sewingProgress;
    public CraftingRecipe currentSewingRecepie;
    public Character sewingMachineUser;
    #endregion
    #endregion
    #endregion

    public GameData() 
    {   // Exists in gamemanager that is.
        #region GameManagerData

        #region TimeData
        clockHour = 6;
        clockMinute = 0;
        dayCount = 0;
        #endregion

        #region DiaryData
        diaryLeftPageIndex = 0;
        diary = new Diary();
        #endregion

        #region EventData
        mainEventIndex = 0;
        #endregion

        #region CharacterData
        
        listOfCharacters = new CharacterList();
        #endregion
        #endregion


        #region Skybox
        dayNightNumber = 0;
        #endregion


        #region InteractableCraftingMachine
        #region CraftingTable
        tableIsCrafting = false;
        craftingProgress = 0;
        currentTableRecepie = null;
        tableUser = null;
        #endregion

        #region Stove
        stoveIsBaking = false;
        bakingInProgress = 0;
        currentBakingRecepie = null;
        stoveUser = null;
        #endregion

        #region Well
        wellIsFetching = false;
        fetchingProgress = 0;
        currentWellRecepie = null;
        wellUser = null;
        #endregion

        #region SewingMachine
        sewingMachineIsSewing = false;
        sewingProgress = 0;
        currentSewingRecepie = null;
        sewingMachineUser = null;
        #endregion
        #endregion
    }
}