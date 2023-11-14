using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        {
            return instance;
        }
    }

    [SerializeField]
    private float DayNightValue = 0; //0.0f - 360.0f
    [SerializeField]
    private float cycleRate = 0.1f;

    private GameObject light;

    private void Awake()
    {
        instance = this;
        light = GameObject.Find("Directional Light");
    }

    private void Update()
    {
        //print("Gecko");
        DayAndNightCycle(cycleRate);
    }

    private void DayAndNightCycle(float rate) 
    {
        float calculatedCycleRate = rate * Time.deltaTime;

        DayNightValue += calculatedCycleRate;

        Quaternion sunRotation = Quaternion.Euler(DayNightValue, 90, 0);

        light.transform.rotation = sunRotation;

        if (DayNightValue >= 360) 
        {
            DayNightValue = 0;
        }
    }
}
