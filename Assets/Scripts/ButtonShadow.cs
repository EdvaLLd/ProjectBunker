using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//Emma
public class ButtonShadow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Shadow shadow;
    bool clicked = false;
    private void Start()
    {
        shadow = GetComponent<Shadow>();
    }

    IEnumerator ShadowTimer(Shadow shadow)
    {
        yield return new WaitForSeconds(0.05f);
        shadow.enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (clicked)
        {
            StartCoroutine(ShadowTimer(shadow));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        shadow.enabled = false;
        clicked = true;
        
    }
}
