using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingWindow : MonoBehaviour
{
    public void ActivateWindow(GameObject windowToOpen)
    {
        windowToOpen.SetActive(!windowToOpen.active);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
