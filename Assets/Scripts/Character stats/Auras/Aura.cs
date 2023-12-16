using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariableModifiers
{
    Health,
    Workspeed,
    Walkspeed
}

public enum Debufftypes
{
    Disease,
    Injury,
    Mood,
    Hungry,
    Immune
}

public class Aura
{
    public Dictionary<VariableModifiers, float> changeValues { get; private set; } = new Dictionary<VariableModifiers, float>();
    public Debufftypes dispelType;
    public bool isTimed { get; private set; }
    public float timer = 0;

    public string ID { get; private set; }
    //tror den här alltid ska vara true?
    public bool isUnique { get; private set; } = true;

    public bool  isSpreadable { get; private set; }
    public float spreadChance;
    public bool makeImmune { get; private set; }

    public bool behaveAsUntreatedWound { get; private set; }
    public bool isTreated;

    MasterAura owner;

    public Statuses status { get; private set; }

    //används bara av unika auras (högt värde är viktigare)
    public int auraPriority { get; private set; }

    public void SetTimed(float timer)
    {
        this.timer = timer;
        isTimed = true;
    }

    public void SetSpreadable(float spreadChance = 0.01f, bool makeImmune = true)
    {
        this.spreadChance = spreadChance;
        isSpreadable = true;
        this.makeImmune = makeImmune;
    }

    public void SetOwner(MasterAura ma)
    {
        owner = ma;
    }

    public void SetAuraPriority(int priority)
    {
        auraPriority = priority;
    }

    public void SetBehaveAsTreatedWound(float timer)
    {
        behaveAsUntreatedWound = true;
        isTreated = false;
        SetTimed(timer);
    }

    public Aura(Debufftypes debuffType, Statuses status)
    {
        dispelType = debuffType;
        ID = IDHandler.GetUniqueID();
        this.status = status;
    }
    public Aura(Debufftypes debuffType, string ID, Statuses status)
    {
        dispelType = debuffType;
        this.ID = ID;
        this.status = status;
    }

    public void GenerateNewID()
    {
        IDHandler.ReturnID(ID);
        ID = IDHandler.GetUniqueID();
    }


    public bool GetValue(VariableModifiers type, out float value)
    {
        if(changeValues.ContainsKey(type))
        {
            value = changeValues[type];
            return true;
        }
        value = 0;
        return false;
    }

    public void AddValueChange(VariableModifiers modifier, float value)
    {
        if(changeValues.ContainsKey(modifier))
        {
            changeValues[modifier] += value;
        }
        else
        {
            changeValues.Add(modifier, value);
        }
    }

    public void AddAura(Aura a)
    {
        foreach (KeyValuePair<VariableModifiers, float> values in a.changeValues)
        {
            AddValueChange(values.Key, values.Value);
        }
    }

    public void RemoveAura(Aura a)
    {
        foreach (KeyValuePair<VariableModifiers, float> values in a.changeValues)
        {
            if(changeValues.ContainsKey(values.Key))
            {
                changeValues[values.Key] -= values.Value;
                if (Mathf.Abs(changeValues[values.Key]) < 0.01f)
                {
                    changeValues.Remove(values.Key);
                }
            }
            else
            {
                Debug.Log("Auras doesnt contain the same values");
            }
        }
        a.OnRemove();
    }

    public void OnApply()
    {

    }

    void OnRemove()
    {
        if(makeImmune && owner != null)
        {
            Aura a = new Aura(Debufftypes.Immune, ID, Statuses.nothing);
            a.SetTimed(60);
            owner.AddAura(a);
            Debug.Log("character is immune to ID " + ID.ToString());
        }
        IDHandler.ReturnID(ID);
    }

    public Aura Clone()
    {
        Aura a = new Aura(dispelType, ID, status);
        a.SetOwner(owner);
        a.AddAura(this);
        if(isSpreadable)
        {
            a.SetSpreadable(spreadChance, makeImmune);
        }
        if(isTimed)
        {
            a.SetTimed(timer);
        }
        SetAuraPriority(auraPriority);
        if(behaveAsUntreatedWound)
        {
            a.SetBehaveAsTreatedWound(timer);
        }
        return a;
    }
}
