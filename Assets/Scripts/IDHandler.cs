using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDHandler
{
    static Dictionary<string, int> ids = new Dictionary<string, int>();

    static int lowestFreeID = -1;

    public static string GetUniqueID()
    {
        string IDToGive = "";
        if(lowestFreeID != -1)
        {
            IDToGive = lowestFreeID.ToString();
            lowestFreeID = -1;
        }
        else
        {
            IDToGive = (ids.Count + 1).ToString();
        }
        ids.Add(IDToGive, 1);
        return IDToGive;
    }

    public static string CopyID(string ID)
    {
        if (ids.ContainsKey(ID))
        {
            ids[ID]++;
        }
        else
        {
            ids.Add(ID, 1);
        }
        return ID;
    }
    public static void ReturnID(string s)
    {
        if(ids.ContainsKey(s))
        {
            ids[s]--;
            if (ids[s] < 1)
            {
                ids.Remove(s);
                GetLowestFreeID();
            }
        }
    }

    static void GetLowestFreeID()
    {
        for (int i = 0; i < ids.Count; i++)
        {
            if(!ids.ContainsKey(i.ToString()))
            {
                lowestFreeID = i;
                return;
            }
        }
    }
}
