using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleVariation : StateMachineBehaviour
{
    [SerializeField] private float timeUntilVariation;
    [SerializeField] private float maxTimeBetweenVariations;
    [SerializeField] private float minTimeBetweenVariations;
    [SerializeField] private int numberOfVariationAnimations;

    private bool readyForVariation;
    private float idleTime;
    private int currentIdleVar;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
        animator.SetFloat("blendIdle", 0);
        currentIdleVar = 0;
        timeUntilVariation = Random.Range(timeUntilVariation, timeUntilVariation + 5);
        //Debug.Log(timeUntilVariation);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (readyForVariation == false)
        {
            idleTime += Time.deltaTime;

            if (idleTime > timeUntilVariation && ((stateInfo.normalizedTime % 1) < 0.02f))
            {
                //Debug.Log("Nu varierar jag mig");
                timeUntilVariation = Random.Range(minTimeBetweenVariations, maxTimeBetweenVariations);
                //Debug.Log("Ny tid: " + timeUntilVariation);

                readyForVariation = true;
                currentIdleVar = Random.Range(1, numberOfVariationAnimations + 1);
                currentIdleVar = currentIdleVar * 2 - 1;

                animator.SetFloat("blendIdle", currentIdleVar - 1);
            } 
        }
        //Kollar om animationen är i slutet av en loop
        else if (stateInfo.normalizedTime % 1 > 0.98f) 
        {
            ResetIdle();
        }

        animator.SetFloat("blendIdle", currentIdleVar, 0.5f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        if (readyForVariation)
        {
            currentIdleVar--;
        }
        readyForVariation = false;
        idleTime = 0;
    }

}
