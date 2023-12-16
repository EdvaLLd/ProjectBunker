using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EqippedGearSet
{
    public Equipment chest = null;
    public Equipment legs = null;
    public Equipment boots = null;
    public Equipment weapon = null;
}

public class GearHandler
{
    EqippedGearSet gearEquipped = new EqippedGearSet();


    public void UnEquipGear(GearTypes gt)
    {
        switch (gt)
        {
            case GearTypes.chest:
                if (gearEquipped.chest != null)
                {
                    Inventory.AddItem(gearEquipped.chest);
                    gearEquipped.chest = null;
                }
                break;
            case GearTypes.legs:
                if (gearEquipped.legs != null)
                {
                    Inventory.AddItem(gearEquipped.boots);
                    gearEquipped.legs = null;
                }
                break;
            case GearTypes.boots:
                if (gearEquipped.boots != null)
                {
                    Inventory.AddItem(gearEquipped.boots);
                    gearEquipped.chest = null;
                }
                break;
            case GearTypes.weapon:
                if (gearEquipped.weapon != null)
                {
                    Inventory.AddItem(gearEquipped.weapon);
                    gearEquipped.weapon = null;
                }
                break;
            default:
                break;
        }
    }

    public void EquipGear(Equipment piece)
    {
        UnEquipGear(piece.gearType);
        switch (piece.gearType)
        {
            case GearTypes.chest:
                gearEquipped.chest = piece;
                break;
            case GearTypes.legs:
                gearEquipped.legs = piece;
                break;
            case GearTypes.boots:
                gearEquipped.boots = piece;
                break;
            case GearTypes.weapon:
                gearEquipped.weapon = piece;
                break;
            default:
                break;
        }
        Inventory.RemoveItem(piece);
    }

    public GearScore GetGearScore()
    {
        GearScore totalScore = new GearScore();
        Equipment e;
        if (GearEquippedInSlot(out e, GearTypes.chest)) totalScore.armor += (e as Armor).GetDefence();
        if (GearEquippedInSlot(out e, GearTypes.legs)) totalScore.armor += (e as Armor).GetDefence();
        if (GearEquippedInSlot(out e, GearTypes.boots)) totalScore.armor += (e as Armor).GetDefence();
        if (GearEquippedInSlot(out e, GearTypes.weapon)) totalScore.attack += (e as Weapon).GetAttack();
        return totalScore;
    }

    public bool GearEquippedInSlot(out Equipment e, GearTypes gt)
    {
        switch (gt)
        {
            case GearTypes.chest:
                e = gearEquipped.chest;
                break;
            case GearTypes.legs:
                e = gearEquipped.legs;
                break;
            case GearTypes.boots:
                e = gearEquipped.boots;
                break;
            case GearTypes.weapon:
                e = gearEquipped.weapon;
                break;
            default:
                e = null;
                break;
        }
        if (e == null) return false;
        return true;
    }
}
