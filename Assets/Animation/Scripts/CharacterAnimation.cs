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
            transform.eulerAngles = new Vector3(0, 180, 0);
        }else if(character.GetCharacterDirectionX() < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        
    }

    public void StartMoving()
    {
        animator.SetBool("isMoving", true);
    }

    public void StopMoving()
    {
        animator.SetBool("isMoving", false);
    }
}
