using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public static bool won = false;

    [SerializeField]
    bool mattersIfWonOrLost = false;
    [SerializeField]
    GameObject winScreen, loseScreen;

    private void Start()
    {
        if(mattersIfWonOrLost)
        {
            winScreen.SetActive(false);
            loseScreen.SetActive(false);
            if (won)
            {
                winScreen.SetActive(true);
            }
            else
            {
                loseScreen.SetActive(true);
            }
        }
    }


    public void OpenScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
