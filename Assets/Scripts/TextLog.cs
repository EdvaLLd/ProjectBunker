using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLog : MonoBehaviour
{
    static GameObject logHolder;
    [SerializeField]
    GameObject logGO;

    static GameObject logGoStatic;

    static List<GameObject> logs = new List<GameObject>();
    void Awake()
    {
        logHolder = GameObject.FindGameObjectWithTag("TextLog");
        logGoStatic = logGO;
    }

    private void Update()
    {
        for (int i = 0; i < logs.Count; i++)
        {
            GameObject item = logs[i];
            Color newColor = item.GetComponent<TextMeshProUGUI>().color;
            newColor.a -= .1f * Time.deltaTime;
            if (newColor.a < 0)
            {
                logs.RemoveAt(i);
                Destroy(item);
            }
            else
            {
                item.GetComponent<TextMeshProUGUI>().color = newColor;
            }
        }
    }

    public static GameObject AddLog(string text, Color color)
    {
        GameObject t = AddLog(text);
        t.GetComponent<TextMeshProUGUI>().color = color;
        return t;
    }

    public static GameObject AddLog(string text)
    {
        GameObject t = Instantiate(logGoStatic, logHolder.transform);
        logs.Add(t);
        t.GetComponent<TextMeshProUGUI>().text = text;
        return t;
    }
}
