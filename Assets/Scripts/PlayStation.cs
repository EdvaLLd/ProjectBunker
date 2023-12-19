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
            //stäng av gamla animations och sånt här
        }
        this.character = character;
        //fixa med nya animaion och sånt här
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
