using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStep : MonoBehaviour
{
    private bool isFinished = false;

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;

            Destroy(this.gameObject);
        }
    }
}
