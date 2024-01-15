using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStation : InteractableItem
{
    Character character;
    [SerializeField]
    float moodChanger = 0.05f;
    public void InteractedWith(Character character)
    {
        if(character != this.character && this.character != null)
        {
            this.character.characterAnim.StopReading();
        }
        this.character = character;
        this.character.characterAnim.Read();
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
            this.character.characterAnim.StopReading();
            this.character = null;
        }
    }
}
