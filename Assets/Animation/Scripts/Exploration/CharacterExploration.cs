using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterExploration : MonoBehaviour
{
    //KOM IHÅG UNIT CONTROLLER INNAN DU PUSHAR GREJER
    
    [SerializeField] private int minExploreTime;
    [SerializeField] private int maxExploreTime;
    private GameManager gameManager;
    private UnityEvent eventActivated = new UnityEvent();

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
        ExplorationEventsElla.ExploreEventTypes mainEvents = new ExplorationEventsElla.ExploreEventTypes();
        bool eventDone = mainEvents.LinnearEventSequence(GetComponent<Character>());
        if (eventDone)
        {
            EndExploration();
        }
        else
        {
            TryActivateLimitedEvent();
        }
    }

    private void TryActivateLimitedEvent()
    {
        ExplorationEventsElla.ExploreEventTypes mainEvents = new ExplorationEventsElla.ExploreEventTypes();
        bool eventDone = mainEvents.LimitedEvent(GetComponent<Character>());
        if (eventDone)
        {
            EndExploration();
        }
        else
        {
            TryActivateRandomEvent();
        }
    }

    private void TryActivateRandomEvent()
    {
        ExplorationEventsElla.ExploreEventTypes mainEvents = new ExplorationEventsElla.ExploreEventTypes();
        bool eventDone = mainEvents.RandomSpecialEvent(GetComponent<Character>());
        if(eventDone)
        {
            EndExploration();
        }
        else
        {
            ActivateStandardEvent();
        }
    }

    private void ActivateStandardEvent()
    {
        Debug.Log("Aktivera standardevent");
        EndExploration();
    }

    private void EndExploration()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;

        for(int i = 0; i < gameManager.randomExploreEvents.Length; i++)
        {
            gameManager.randomExploreEvents[i].IncreaseTurnsSinceActivated();
        }
        for(int i = 0; i < gameManager.limitedExploreEvents.Length; i++)
        {
            gameManager.limitedExploreEvents[i].IncreaseTurnsSinceActivated();
        }
    }

    private IEnumerator ExploreWait()
    {
        yield return new WaitForSeconds(Random.Range(minExploreTime, maxExploreTime));
        TryActivateMainEvent();
    }
}
