using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Emma
public class ButtonShadow : MonoBehaviour
{
    public void StopShadow(Shadow shadow)
    {
        shadow.enabled = false;
        StartCoroutine(ShadowTimer(shadow));
    }

    IEnumerator ShadowTimer(Shadow shadow)
    {
        yield return new WaitForSeconds(0.05f);
        shadow.enabled = true;
    }
}
