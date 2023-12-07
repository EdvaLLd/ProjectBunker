using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Desease
{
    private float health = 100; //procent innan sjukdomen försvinner

    protected Character characterAffected;

    protected int deseaseProgress = 0; //om man vill rampa upp sjukdomen över tid



    public void SetCharacter(Character c)
    {
        characterAffected = c;
    }

    abstract public void Tick();
    abstract protected void Affect<T>() where T : Desease, new();
    virtual public void Medicate()
    {
        ChangeDeseaseHealth(-100);
    }

    virtual protected void Recover() { }

    protected virtual void ChangeDeseaseHealth(float changeValue)
    {
        health += changeValue;
        if(health < 0)
        {
            characterAffected.RemoveDesease(this);
            Debug.Log("cured!");
        }
    }

    public void RefreshDesease()
    {
        TextLog.AddLog("decease refreshed");
        health = 100;
    }

    public float GetHealth()
    {
        return health;
    }
}
