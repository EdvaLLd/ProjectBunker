using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterExplorationUI : MonoBehaviour
{
    static GameObject UI;
    static Character c;
    public static bool foundParadise;
    [SerializeField]
    GameObject paradiseBtn;
    static GameObject paradiseBtnStatic;

    private void Start()
    {
        paradiseBtnStatic = paradiseBtn;
        UI = GameObject.FindGameObjectWithTag("ExplorationUI");
        UIManager.CloseWindow(UI);
    }


    public static void OpenUI(Character character)
    {
        c = character;
        UIManager.SetWindowActive(UI);
        paradiseBtnStatic.SetActive(foundParadise);
    }

    public void ButtonClicked(int nr)
    {
        UnitController.SendToExplore(nr, c);
        UIManager.CloseWindow(UI);
    }
    public void GoToParadise()
    {
        MainMenu.won = true;
        SceneManager.LoadScene("ParadiseScene");
    }
}
