using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStation : InteractableItem
{
    Character character;
    [SerializeField]
    float moodChanger = 0.1f;
    public void InteractedWith(Character character)
    {
        if(character != this.character)
        {
            //st�ng av gamla animations och s�nt h�r
        }
        this.character = character;
        //fixa med nya animaion och s�nt h�r
    }

    private void Update()
    {
        if(character != null)
        {
            character.AddMood(moodChanger * Time.deltaTime);
        }
    }

    public void CharacterLeftStation(Character character)
    {
        if(this.character == character)
        {
            this.character = null;
        }
    }
}
