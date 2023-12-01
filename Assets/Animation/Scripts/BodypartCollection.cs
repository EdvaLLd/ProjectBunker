using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartCollection
{
    private int[][] bodyParts = { new int[] {14, 21, 12}, new int[] {15, 22, 13 }, new int[] {3, 4, 5, 6, 7, 8, 9, 10 }, 
        new int[] {0, 1, 2 }, new int[] {18, 19, 16 }, new int[] {17, 20 }, new int[] {11} };
    private int[] versionNumbers = new int[7];

    //Skinparts ------- Torso --------- Ögon --------- Mun ------- Ben -------- Fötter ---------- Hår

    public BodyPartCollection(int[] ogVersionNumbers) 
    { 
        for(int i = 0; i < versionNumbers.Length; i++)
        {
            versionNumbers[i] = ogVersionNumbers[i];
        }
    }

    public void ChangePants(int newVersion)
    {
        versionNumbers[4] = newVersion;
    }

    public void ChangeShirt(int newVersion)
    {
        versionNumbers[1] = newVersion;
    }
    public void ChangeShoes(int newVersion)
    {
        versionNumbers[5] = newVersion;
    }

    public int[] GetVersionNumbers()
    {
        return versionNumbers;
    }
   
}
