using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flu : SpreadableDesease
{
    public Flu()
    {
        SetSpreadFrequency(1f);
        SetSpreadChance(0.05f);
    }
    public override void Tick()
    {
        Affect<Flu>();
        Recover();
        //Debug.Log(GetHealth());
    }
    protected override void Affect<T>()
    {
        base.Affect<T>();
        characterAffected.TakeDamage(.5f * Time.deltaTime);
    }

    protected override void Recover()
    {
        ChangeDeseaseHealth(-2 * Time.deltaTime);
    }
}
