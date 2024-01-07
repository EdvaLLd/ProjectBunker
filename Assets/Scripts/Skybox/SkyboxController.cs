using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SkyboxController : MonoBehaviour,IDataPersistance
{
    [SerializeField] Transform _Sun = default;
    [SerializeField] Transform _Moon = default;
    private enum moonPhases { New_moon, Waxing_crescent, First_quarter, Waxing_gibbous, Full_moon, Waning_gibbous, Third_quarter, Waning_crescent };
    [SerializeField] private moonPhases moonPhase;
    /*[SerializeField]*/ private Vector3 MoonPhaseVector; // A bit funky right now but we'll se how it goes.

   
    [Tooltip("Min=0, Max=360, 0=dawn, 90=day, 180=dusk, 270=night.")]
    public float dayNightValue = 0; //0.0f - 360.0f
    public float cycleRate = 1;

    private void CycleMoonPhase() 
    {

    }

    public void DayAndNightCycle(float rate/*, float moonAngle*/)
    {
        float calculatedCycleRate = rate * Time.deltaTime;

        dayNightValue += calculatedCycleRate;

        _Sun.transform.rotation = Quaternion.Euler(dayNightValue, -38, 0);
        _Moon.transform.rotation = Quaternion.Euler(dayNightValue + 180 /*+ moonAngle*/, -38, 0);

        /* if (sun.transform.rotation.x == 30)
         {

         }*/

        if (dayNightValue >= 360)
        {
            dayNightValue = 0;
        }

        SetMoonPhase(moonPhase);
    }

    private void SetMoonPhase(moonPhases phase) 
    {
        switch (phase) 
        {
            case (moonPhases.New_moon): MoonPhaseVector = new Vector3(0,0,0);
                break;
            case (moonPhases.Waxing_crescent): MoonPhaseVector = new Vector3(-10,0,0);
                break;
            case (moonPhases.First_quarter): MoonPhaseVector = new Vector3(-2,0,0);
                break;
            case (moonPhases.Waxing_gibbous): MoonPhaseVector = new Vector3(0.3f,0,0);
                break;
            case (moonPhases.Full_moon): MoonPhaseVector = new Vector3(1,1,1);
                break;
            case (moonPhases.Waning_gibbous): MoonPhaseVector = new Vector3(1,1,-0.15f);
                break;
            case (moonPhases.Third_quarter): MoonPhaseVector = new Vector3(1,1,-0.5f);
                break;
            case (moonPhases.Waning_crescent): MoonPhaseVector = new Vector3(1,1,-1.5f);
                break;

            default:
                break;
        }
    }

    public void LoadData(GameData data)
    {
        dayNightValue = data.dayNightNumber;
    }

    public void SaveData(ref GameData data)
    {
        data.dayNightNumber = dayNightValue;
    }

    void LateUpdate()
    {
        // Directions are defined to point towards the object

        // Sun
        Shader.SetGlobalVector("_SunDir", -_Sun.transform.forward);

        // Moon
        Shader.SetGlobalVector("_MoonDir", /*temp*/-_Moon.transform.forward);

        Shader.SetGlobalVector("_MoonPhaseMask", MoonPhaseVector/*-_Moon.transform.forward*/);
    }
}
