using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    //[SerializeField] private GameObject ghost;
    [SerializeField] private GameObject guitar;
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject ghost;

    private Character character;
    private Animator animator;

    void Start()
    {
        character = GetComponentInParent<Character>();
        animator = GetComponent<Animator>();
        //ghost.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            PlayGuitar();
        }else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Read();
        }else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Die();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            ShowGhost();
        }else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            StopPlayingGuitar();
            StopReading();
        }
    }

    public void Flip()
    {
        if(character.GetCharacterDirectionX() > 0)
        {
            StopClimbing();
            transform.eulerAngles = new Vector3(0, 180, 0);
        }else if(character.GetCharacterDirectionX() < 0)
        {
            StopClimbing();
            transform.eulerAngles = new Vector3(0, 0, 0);
        }else if(character.GetCharacterDirectionX() == 0)
        {
            StartClimbing();
        }
        
    }

    public void PlayGuitar()
    {
        animator.SetBool("isPlayingGuitar", true);
        animator.SetBool("isHappy", true);
        guitar.SetActive(true);
    }

    public void StopPlayingGuitar()
    {
        animator.SetBool("isPlayingGuitar", false);
        animator.SetBool("isHappy", false);
        guitar.SetActive(false);
    }

    public void Read()
    {
        animator.SetBool("isReading", true);
        animator.SetBool("isHappy", true);
        book.SetActive(true);
    }

    public void StopReading()
    {
        animator.SetBool("isReading", false);
        animator.SetBool("isHappy", false);
        book.SetActive(false);
    }

    public void StartMoving()
    {
        animator.SetTrigger("isNeutral");
        animator.SetBool("isMoving", true);
    }

    public void StopMoving()
    {
        animator.SetBool("isMoving", false);
    }

    public void Die()
    {
        animator.SetTrigger("died");
    }

    //Ansiktsanimationer kommer troligtvis få egna metoder sen, men det här är bara för testning
    public void StartCrafting()
    {
        animator.SetTrigger("isHappy");
        animator.SetBool("isCrafting", true);
    }

    public void StopCrafting()
    {
        animator.SetTrigger("isNeutral");
        animator.SetBool("isCrafting", false);
    }

    public void ShowGhost()
    {
        ghost.SetActive(true);
    }

    //public void Loot()
    //{
    //    animator.SetTrigger("loot");
    //}

    private void StartClimbing()
    {
        animator.SetBool("isClimbing", true);

        //Om vi skulle vilja ha en skillnad mellan upp och ner
        animator.SetFloat("climbingDirection", character.GetCharacterDirectionY());
    }

    private void StopClimbing()
    {
        animator.SetBool("isClimbing", false);
    }

}
