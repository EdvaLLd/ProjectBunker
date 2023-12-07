using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpreadableDesease : Desease
{
    private float spreadDist = 5;
    private float spreadFrequency = 10;
    private float spreadFrequencyMax;
    private float spreadChance = 0.1f;

    protected SpreadableDesease()
    {
        spreadFrequencyMax = spreadFrequency;
    }

    protected void SetSpreadDist(float value)
    {
        spreadDist = value;
    }

    protected void SetSpreadFrequency(float value)
    {
        spreadFrequency = value;
        spreadFrequencyMax = value;
    }

    protected void SetSpreadChance(float value)
    {
        spreadChance = value;
    }

    protected override void Affect<T>()
    {
        spreadFrequency -= Time.deltaTime;
        if (spreadFrequency < 0)
        {
            spreadFrequency = spreadFrequencyMax;
            Spread<T>();
        }
    }
    private void Spread<T>() where T : Desease, new()
    {
        List<Character> allCharacters = UnitController.GetCharacters();
        List<Character> allViableVictims = new List<Character>();
        foreach (Character c in allCharacters)
        {
            if(!HelperMethods.WallBetweenPoints(characterAffected.transform.position, c.transform.position) &&
                Vector3.Distance(characterAffected.transform.position, c.transform.position) < spreadDist &&
                c != characterAffected)
            {
                allViableVictims.Add(c);
            }
        }
        if(allViableVictims.Count > 0 && Random.Range(0f,1f) < spreadChance)
        {
            Character randomPerson = allViableVictims[Random.Range(0, allViableVictims.Count)];
            //T d;
            /*if(randomPerson.HasDesease(out d))
            {
                d.RefreshDesease();
            }
            else
            {
                randomPerson.AddDesease<T>();
            }*/
            randomPerson.AddDesease<T>();
        }
    }
}
