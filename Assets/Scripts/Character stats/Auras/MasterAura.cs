using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterAura
{
    public Aura aura { get; private set; } = new Aura(Debufftypes.Immune, Statuses.nothing);

    Dictionary<Debufftypes, List<Aura>> auras = new Dictionary<Debufftypes, List<Aura>>();
    Dictionary<Debufftypes, Aura> uniqueDebuffTypes = new Dictionary<Debufftypes, Aura>();
    HashSet<Statuses> statuses = new HashSet<Statuses>();
    Character character;

    float timer = 0;

    public MasterAura(Character c)
    {
        character = c;
    }

    bool TypeIsUnique(Debufftypes t)
    {
        if(t == Debufftypes.Mood)
        {
            return true;
        }
        return false;
    }

    public void AddAura(Aura a)
    {
        if (!TypeIsUnique(a.dispelType))
        {
            if (!a.isUnique || !HasAuraWithID(a.ID) || a.dispelType == Debufftypes.Immune)
            {
                if (auras.ContainsKey(a.dispelType))
                {
                    auras[a.dispelType].Add(a);
                }
                else
                {
                    auras.Add(a.dispelType, new List<Aura>() { a });
                }
                ApplyAura(a);
            }
        }
        else
        {
            if(uniqueDebuffTypes.ContainsKey(a.dispelType))
            {
                if (uniqueDebuffTypes[a.dispelType].auraPriority > a.auraPriority) return;
                aura.RemoveAura(uniqueDebuffTypes[a.dispelType]);
                uniqueDebuffTypes.Remove(a.dispelType);
                UpdateStatuses();
            }
            ApplyAura(a);
            uniqueDebuffTypes[a.dispelType] = a;
        }
    }

    void ApplyAura(Aura a)
    {
        statuses.Add(a.status);
        aura.AddAura(a);
        a.SetOwner(this);
        a.OnApply();
        character.AuraValuesChanged();
    }

    public bool RemoveUniqueAura(Debufftypes debuffType)
    {
        if(uniqueDebuffTypes.ContainsKey(debuffType))
        {
            aura.RemoveAura(uniqueDebuffTypes[debuffType]);
            uniqueDebuffTypes.Remove(debuffType);
            UpdateStatuses();
            character.AuraValuesChanged();
            return true;
        }
        return false;
    }

    public bool RemoveAuras(Debufftypes dispelType)
    {
        if (TypeIsUnique(dispelType))
        {
            RemoveUniqueAura(dispelType);
            return true;
        }
        else
        {
            if (auras.ContainsKey(dispelType))
            {
                bool auraTreated = false;
                bool removeList = true;
                foreach (Aura item in auras[dispelType])
                {
                    if (item.behaveAsUntreatedWound && !item.isTreated)
                    {
                        item.isTreated = true;
                        removeList = false;
                        auraTreated = true;
                        TextLog.AddLog(character.characterName + " had her wounds treated and heals quickly!");
                    }
                    else if(!item.behaveAsUntreatedWound)
                    {
                        aura.RemoveAura(item);
                        auraTreated = true;
                    }
                }
                if (removeList)
                {
                    auras.Remove(dispelType);
                }
                if(auraTreated)
                {
                    UpdateStatuses();
                    character.AuraValuesChanged();
                }
                
                return auraTreated;
            }
            return false;
        }
    }

    public bool HasAuraType(Debufftypes type)
    {
        return auras.ContainsKey(type) || uniqueDebuffTypes.ContainsKey(type);
    }

    public bool HasAuraWithStatus(Statuses status)
    {
        return statuses.Contains(status);
    }

    void UpdateStatuses()
    {
        statuses.Clear();
        List<Aura> allAuras = GetAllAuras();
        foreach (Aura a in allAuras)
        {
            statuses.Add(a.status);
        }
    }

    bool HasAuraWithID(string ID)
    {
        foreach (KeyValuePair<Debufftypes, List<Aura>> item in auras)
        {
            foreach (Aura a in item.Value)
            {
                if(a.ID == ID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Aura> GetAllAuras()
    {
        List<Aura> allAuras = new List<Aura>();
        foreach (KeyValuePair<Debufftypes, List<Aura>> item in auras)
        {
            foreach (Aura a in item.Value)
            {
                allAuras.Add(a);
            }
        }
        foreach (KeyValuePair<Debufftypes, Aura> a in uniqueDebuffTypes)
        {
            allAuras.Add(a.Value);
        }
        return allAuras;
    }

    public void Tick()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            Damage();
            LoopThroughAuras();
            timer = 0;
        }
    }



    //de här kollas för alla auras och händer ungefär 1 gång i sekunden
    //GENERAL AURA TICKS
    void Damage()
    {
        float damage;
        if (aura.GetValue(VariableModifiers.Health, out damage))
        {
            character.TakeDamage(-damage * timer);
        }
    }

    void LoopThroughAuras()
    {
        List<Aura> aurasToRemove = new List<Aura>();
        List<Character> charactersInRoom = HelperMethods.GetCharactersInSameRoom(character);
        List<Aura> allAuras = GetAllAuras();
        foreach (Aura a in allAuras)
        {
            if (a.isTimed
                && (!a.behaveAsUntreatedWound || a.isTreated))
            {
                a.timer -= timer;
                if (a.timer < 0)
                {
                    aurasToRemove.Add(a);
                }
            }
            if (a.isSpreadable)
            {
                if (charactersInRoom.Count > 0 && Random.Range(0f, 1f) < a.spreadChance)
                {
                    charactersInRoom = HelperMethods.ScrambleList(charactersInRoom);
                    for (int i = 0; i < charactersInRoom.Count; i++)
                    {
                        if (!charactersInRoom[i].masterAura.HasAuraWithID(a.ID))
                        {
                            charactersInRoom[i].masterAura.AddAura(a.Clone());
                            break;
                        }
                    }
                }
            }
        }
        if (aurasToRemove.Count > 0)
        {
            RemoveAuras(aurasToRemove);
        }
    }
    void RemoveAuras(List<Aura> aurasToRemove)
    {
        for (int i = aurasToRemove.Count - 1; i >= 0; i--)
        {
            aura.RemoveAura(aurasToRemove[i]);
            if (TypeIsUnique(aurasToRemove[i].dispelType))
            {
                uniqueDebuffTypes.Remove(aurasToRemove[i].dispelType);
            }
            else
            {
                auras[aurasToRemove[i].dispelType].Remove(aurasToRemove[i]);
                if (auras[aurasToRemove[i].dispelType].Count == 0)
                {
                    auras.Remove(aurasToRemove[i].dispelType);
                }
            }
        }
        UpdateStatuses();
        character.AuraValuesChanged();
    }
}
