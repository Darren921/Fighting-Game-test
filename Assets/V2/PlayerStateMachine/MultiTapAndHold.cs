using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MultiTapAndHold : IInputInteraction
{
    //Above this value a tap is pressed
    public float pressPoint = 0.4f;

    //Below this value a tap is released
    public float releasePoint = 0.2f;

    //Number of taps to perform the multi tap
    public float multiTapCount=2;

    //"Time in sceonds to complete your multi tap before the action is canceled
    public float duration = 0.5f;

    float tapCounter;


    static MultiTapAndHold()
    {
        InputSystem.RegisterInteraction<MultiTapAndHold>();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {

    }
    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }
        var actuation = context.ComputeMagnitude();
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
                    tapCounter++;
                    if (tapCounter >= multiTapCount)
                    {
                        context.PerformedAndStayPerformed();
                    }
                }
                break;

            case InputActionPhase.Performed:
                if (!context.ControlIsActuated())
                {
                    context.Canceled();
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
    }
}
