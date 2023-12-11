using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterExploration : MonoBehaviour
{
    //KOM IHÅG UNIT CONTROLLER INNAN DU PUSHAR GREJER
    
    [SerializeField] private int minExploreTime;
    [SerializeField] private int maxExploreTime;
    private bool eventActivated = false;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public void StartExploration()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(ExploreWait());
    }
    private void TryActivateMainEvent()
    {
        if (GetComponent<ExplorationEventsElla.ExploreEventTypes>().LinnearEventSequence())
        {
            eventActivated = true;
        }
        else
        {
            eventActivated = false;
        }
    }

    private void TryActivateRandomEvent()
    {

    }

    private void ActivateStandardEvent()
    {

    }

    private void ResetValues()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private IEnumerator ExploreWait()
    {
        yield return new WaitForSeconds(Random.Range(minExploreTime, maxExploreTime));
        TryActivateMainEvent();
    }
}
