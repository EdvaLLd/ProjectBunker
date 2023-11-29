using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Character character;
    private Animator animator;

    void Start()
    {
        character = GetComponentInParent<Character>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

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

    public void StartMoving()
    {
        animator.SetTrigger("isNeutral");
        animator.SetBool("isMoving", true);
    }

    public void StopMoving()
    {
        animator.SetBool("isMoving", false);
    }

    private void StartClimbing()
    {
        animator.SetBool("isClimbing", true);
    }

    private void StopClimbing()
    {
        animator.SetBool("isClimbing", false);
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

    //public void Loot()
    //{
    //    animator.SetTrigger("loot");
    //}

}
