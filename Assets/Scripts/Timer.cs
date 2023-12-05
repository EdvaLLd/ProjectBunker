using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer/* : MonoBehaviour*/
{
    public enum timeUnits { second, minute, hour };
    public timeUnits timeUnit;
    public float ammount;

    public IEnumerator CountDown()
    {
        if (ammount > 0) 
        {
            switch (timeUnit) 
            {
                case (timeUnits.second):
                    yield return new WaitForSeconds(ammount);
                    break;
                case (timeUnits.minute):
                    yield return new WaitForSeconds(ammount*60);
                    break;
                case (timeUnits.hour):
                    yield return new WaitForSeconds(ammount*360);
                    break;

                default:
                    break;
            }
        }
    }
}
