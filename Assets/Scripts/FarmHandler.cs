using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmHandler : MonoBehaviour
{
    public static Farming activeInstance;
    public void AddSlot()
    {
        if(activeInstance != null)
        {
            activeInstance.AddSlot();
        }
    }
}
