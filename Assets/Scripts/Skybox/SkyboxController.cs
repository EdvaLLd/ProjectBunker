using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SkyboxController : MonoBehaviour
{
    [SerializeField] Transform _Sun = default;
    [SerializeField] Transform _Moon = default;
    [SerializeField] private Vector3 MoonPhaseVector; // A bit funky right now but we'll se how it goes.

   
    [Tooltip("Min=0, Max=360, 0=dawn, 180=dusk.")]
    public float dayNightValue = 0; //0.0f - 360.0f
    public float cycleRate = 1;

    private void CycleMoonPhase() 
    {

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
