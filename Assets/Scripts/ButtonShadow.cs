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

    bool buttonActive = true;

    private void Start()
    {
        shadow = GetComponent<Shadow>();
        UIManager.onButtonDisableChanged += ButtonDisabledChanged;
    }

    void ButtonDisabledChanged(Button button)
    {
        if(button == GetComponent<Button>())
        {
            buttonActive = button.enabled;
            GetComponent<Shadow>().enabled = buttonActive;
        }
    }

    IEnumerator ShadowTimer(Shadow shadow)
    {
        yield return new WaitForSeconds(0.05f);
        if (buttonActive)
        {
            shadow.enabled = true;
            Animator anim;
            if (TryGetComponent<Animator>(out anim))
            {
                anim.SetTrigger("TriggerRelease");
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (clicked && buttonActive)
        {
            StartCoroutine(ShadowTimer(shadow));

           
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonActive)
        {
            shadow.enabled = false;
            clicked = true;

            Animator anim;
            if (TryGetComponent<Animator>(out anim))
            {
                anim.SetTrigger("TriggerPress");
            }
        } 
    }
}
