using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class SelectionVisibilityModifier
{
    public Material material;
}

public class UnitController : MonoBehaviour
{
    //Hur man ser skillnad om en karaktär är markerad eller inte
    [SerializeField]
    SelectionVisibilityModifier unSelectedModifierSetter, selectedModifierSetter;

    //Ska kanske finnas en speedmodifier på varje karaktär?
    [SerializeField]
    float movementSpeedSetter = 1;


    //Lägg till alla serializedfield-variabler här som static och i start
    static SelectionVisibilityModifier unSelectedModifier, selectedModifier;
    public static float movementSpeed; 


    static Character selectedCharacter = null;

    public static InteractableItem itemInteractedWith = null;

    private void Start()
    {
        unSelectedModifier = unSelectedModifierSetter;
        selectedModifier = selectedModifierSetter;
        movementSpeed = movementSpeedSetter;
    }

    private void Update()
    {
        if(itemInteractedWith != null && selectedCharacter != null)
        {
            selectedCharacter.InteractedWithItem(itemInteractedWith);
            itemInteractedWith = null;
        }
    }


    public static void SwapSelectedCharacter(Character newSelectedCharacter)
    {
        if (selectedCharacter == newSelectedCharacter)
        {
            setCharacterVisual(selectedCharacter, false);
            selectedCharacter = null;
        }
        else
        {
            if (selectedCharacter != null)
            {
                setCharacterVisual(selectedCharacter, false);
            }
            selectedCharacter = newSelectedCharacter;
            setCharacterVisual(selectedCharacter, true);
        }
    }

    public static void setCharacterVisual(Character character, bool isSelected)
    {
        if(isSelected)
        {
            character.GetComponent<MeshRenderer>().material = selectedModifier.material;
        }
        else
        {
            character.GetComponent<MeshRenderer>().material = unSelectedModifier.material;
        }
    }
}
