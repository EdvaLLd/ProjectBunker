using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flu : SpreadableDesease
{
    public Flu()
    {
        SetSpreadFrequency(.1f);
        SetSpreadChance(0.01f);
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
        characterAffected.TakeDamage(3 * Time.deltaTime);
    }

    protected override void Recover()
    {
        ChangeDeseaseHealth(-5 * Time.deltaTime);
    }
}
