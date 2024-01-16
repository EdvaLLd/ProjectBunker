using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public GameObject guitar;
    public GameObject book;
    [SerializeField] private GameObject ghost;

    private Character character;
    private Animator animator;
    private PartsChanger partChanger;

    void Start()
    {
        character = GetComponentInParent<Character>();
        animator = GetComponent<Animator>();
        partChanger = GetComponent<PartsChanger>();
        ghost.SetActive(false);
        book.SetActive(false);
        guitar.SetActive(false);
    }

    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Alpha1))
        //{
        //    TurnSick();
        //}
        //if (Input.GetKeyUp(KeyCode.Alpha2))
        //{
        //    BeCured();
        //}

        //}else if (Input.GetKeyUp(KeyCode.Alpha3))
        //{
        //    Die();
        //}
        //else if (Input.GetKeyUp(KeyCode.Alpha4))
        //{
        //    ShowGhost();
        //}else 
        //if (Input.GetKeyUp(KeyCode.Alpha5))
        //{
        //    StopReading();
        //    PlayGuitar();
        //} else if (Input.GetKeyUp(KeyCode.Alpha6))
        //{
        //    StopPlayingGuitar();
        //    Read();
        //}
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
        animator.SetBool("isMoving", true);
    }

    public void StopMoving()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isClimbing", false);
    }

    public void Die()
    {
        animator.SetTrigger("died");
    }

    public void StartCrafting()
    {
        animator.SetBool("isCrafting", true);
    }

    public void StopCrafting()
    {
        animator.SetBool("isCrafting", false);
    }

    public void ShowGhost()
    {
        ghost.SetActive(true);
    }

    public void TurnSick()
    {
        partChanger.ChangeFaceColor(new Color(0.5666106f, 0.7830189f, 0.4912336f));
    }

    public void BeCured()
    {
        partChanger.ChangeFaceColor(new Color(1, 1, 1));
    }

    public void BeSad()
    {
        animator.SetBool("isSad", true);
    }

    public void BeHappy()
    {
        animator.SetBool("isHappy", true);
    }

    public void BeNeutral()
    {
        animator.SetBool("isHappy", false);
        animator.SetBool("isSad", false);

    }

    public void ChangeEquipment(GearTypes type, int spriteID)
    {
        switch (type)
        {
            case GearTypes.chest:
                partChanger.ChangeShirt(spriteID);
                break;
            case GearTypes.legs:
                partChanger.ChangePants(spriteID);
                break;
            case GearTypes.boots: 
                partChanger.ChangeShoes(spriteID);
                break;
            case GearTypes.weapon:
                return;
        }
    }

    public void RemoveEquipment(GearTypes type)
    {
        switch (type)
        {
            case GearTypes.chest:
                partChanger.RemoveShirt();
                break;
            case GearTypes.legs:
                partChanger.RemovePants();
                break;
            case GearTypes.boots:
                partChanger.RemoveShoes();
                break;
            case GearTypes.weapon:
                return;
        }
    }

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
