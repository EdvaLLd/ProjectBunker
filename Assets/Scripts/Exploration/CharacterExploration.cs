using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterExploration : MonoBehaviour
{
    
    [SerializeField] private int minExploreTime;
    [SerializeField] private int maxExploreTime;
    private GameManager gameManager;
    [SerializeField] private AudioClip clip;
    private AudioSource audioSource;
    private Character character;

    private int locationNr;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        if(clip != null)
        {
            audioSource.clip = clip;
        }
        character = GetComponent<Character>();
    }

    private void Start()
    {
        
    }
    public void StartExploration(int location)
    {
        locationNr = location;
        TextLog.AddLog(character.characterName + " went exploring.");
        //transform.GetChild(0).gameObject.SetActive(false);
        //transform.GetChild(2).gameObject.SetActive(false);
        transform.position -= new Vector3(0,0,100);
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(ExploreWait());
        List<Character> characters = UnitController.GetCharacters();
        foreach (Character c in characters)
        {
            if(c.task != CharacterTasks.exploring)
            {
                return;
            }
        }
        Time.timeScale = 3;
    }
    private void TryActivateMainEvent()
    {
        ExplorationBase.ExploreEventTypes baseEvent = new ExplorationBase.ExploreEventTypes();
        bool eventDone = baseEvent.LinnearEventSequence(GetComponent<Character>());
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
        ExplorationBase.ExploreEventTypes baseEvent = new ExplorationBase.ExploreEventTypes();
        bool eventDone = baseEvent.LimitedEvent(GetComponent<Character>());
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
        ExplorationBase.ExploreEventTypes baseEvent = new ExplorationBase.ExploreEventTypes();
        bool eventDone = baseEvent.RandomSpecialEvent(GetComponent<Character>());
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
        //Random.Range(0, gameManager.standardExploreEvents.Length)
        ExplorationBase.ExploreEventTypes.SimpleLootEvent[] lootEvents =  gameManager.standardExploreEvents[locationNr].loot;
        for(int i = 0; i < lootEvents.Length; i++)
        {
            int randomAmount = Random.Range(lootEvents[i].minAmount, lootEvents[i].maxAmount +1);
            if(randomAmount > 0)
            {
                Inventory.AddItem(lootEvents[i].item, randomAmount);
            }     
        }
        EndExploration();
    }

    private void EndExploration()
    {
        TextLog.AddLog(character.characterName + " came back from their adventure.");
        //transform.GetChild(0).gameObject.SetActive(true);
        //transform.GetChild(2).gameObject.SetActive(true);
        transform.position += new Vector3(0, 0, 100);
        GetComponent<Character>().task = CharacterTasks.none;
        GetComponent<BoxCollider2D>().enabled = true;
        audioSource.Play();

        for(int i = 0; i < gameManager.randomExploreEvents.Length; i++)
        {
            gameManager.randomExploreEvents[i].IncreaseTurnsSinceActivated();
        }
        for(int i = 0; i < gameManager.limitedExploreEvents.Length; i++)
        {
            gameManager.limitedExploreEvents[i].IncreaseTurnsSinceActivated();
        }

        Time.timeScale = 1;
    }

    private IEnumerator ExploreWait()
    {
        yield return new WaitForSeconds(Random.Range(minExploreTime, maxExploreTime));
        TryActivateMainEvent();
    }
}
