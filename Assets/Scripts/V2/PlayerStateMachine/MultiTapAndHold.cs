using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MultiTapAndHold : IInputInteraction
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


    static MultiTapAndHold()
    {
        InputSystem.RegisterInteraction<MultiTapAndHold>();
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
                }
                break;

            case InputActionPhase.Started:
                if (context.ControlIsActuated(pressPoint))
                {
                    holdTime += Time.deltaTime;
                    Debug.Log(holdTime);
                    tapCounter++;
                    if (tapCounter >= multiTapCount && holdTime <= 0.0257)
                    {
                        context.PerformedAndStayPerformed();
                    }
                }
                break;

            case InputActionPhase.Performed:
                if (!context.ControlIsActuated())
                {
                    context.Canceled();
//                    Debug.Log("Cancelled actuated ");
                }
                if(!context.ControlIsActuated(releasePoint))
                {
                    context.Canceled();

                }
                break;

            case InputActionPhase.Canceled:
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
