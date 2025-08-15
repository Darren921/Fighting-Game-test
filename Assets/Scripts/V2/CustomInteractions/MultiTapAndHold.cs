using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MultiTapOrHold : IInputInteraction
{
    [Tooltip("Number of taps to perform the multi tap")]
    public float multiTapCount=2;
    
    [Tooltip("Below this value a tap is released")]
    public float releasePoint = 0.2f;
    
    [Tooltip("Time in seconds to complete your multi tap before the action is canceled")]
    public float duration = 0.5f;

    [Tooltip("Above this value a tap is pressed")]
    public float pressPoint = 0.4f;

    float tapCounter;
    private float holdTime;
    internal bool holding;
    private float currentTime;


    static MultiTapOrHold()
    {
        InputSystem.RegisterInteraction<MultiTapOrHold>();
    }
    
    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }
        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.ControlIsActuated(pressPoint))
                {
                    tapCounter++;
                    context.Started();
                    context.SetTimeout(duration + 0.00001f); 
                    currentTime = Time.time;
//                    Debug.Log("waited");
                }
                break;

            case InputActionPhase.Started:
                if (context.ControlIsActuated(pressPoint))
                {
                    holdTime = Time.time - currentTime;
//                    Debug.Log(holdTime);
                    tapCounter++;
             //       Debug.Log("started");
                    if (tapCounter >= multiTapCount && holdTime < 0.189)
                    {
                        if (holdTime > 0.185 && !holding)
                        {
                            context.Canceled();
                        }
                        holding = false;
                        context.Performed();
                    }
                    else if (tapCounter >= multiTapCount && holdTime >= 0.189)
                    {
                        holding = true;
                        context.PerformedAndStayPerformed();
                    }
                }
                break;

            case InputActionPhase.Performed:
                Debug.Log("performed" + holdTime + " seconds");

                if (!context.ControlIsActuated())
                {
                    context.Canceled();
//               Debug.Log("Cancelled actuated 1 ");
                }
                if(!context.ControlIsActuated(releasePoint))
                {
                    context.Canceled();
//                    Debug.Log("Cancelled actuated 2");

                }
                break;

            case InputActionPhase.Canceled:
                Debug.Log("canceled");
                Reset();
                break;

        }
    }

    public void Reset()
    {
        tapCounter = 0;
        holdTime = 0;
    }
}
