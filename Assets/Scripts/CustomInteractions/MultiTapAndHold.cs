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
    public float totalDuration = 0.5f;
    [Tooltip("Time in seconds to increase tap count before the action is canceled")]
    public float tapDuration  = 0.5f;
    [Tooltip("Above this value a tap is pressed")]
    public float pressPoint = 0.4f;

    private float _tapCounter;
    private float _holdTime = 0.1f;
    internal bool Holding;
    private double _currentTapTime;
    private double _currentReleaseTime;


    static MultiTapOrHold()
    {
        InputSystem.RegisterInteraction<MultiTapOrHold>();
    }
    
    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            Reset();
            return;
        }

        switch (currentPhase)
        {
            case Phase.None:
            {
                if (context.ControlIsActuated(pressPoint))
                {
                    _tapCounter++;
                    currentPhase = Phase.WaitForRelease;
                    _currentTapTime = context.time;
                    context.Started();
                    context.SetTimeout(totalDuration);
                    Debug.Log("MultiTapOrHold: Started");
                }

                break;
            }
            case Phase.Tap:
                
                _tapCounter++;
                if (_tapCounter >= multiTapCount)
                {
                    currentPhase = Phase.Performed;
                    context.PerformedAndStayPerformed();
                    Debug.Log("MultiTapOrHold: Performed");
                }
                else
                {
                    currentPhase = Phase.WaitForRelease;
                }
                break;
            case Phase.WaitForRelease:
                if (!context.ControlIsActuated(releasePoint))
                {
                    if (context.time - _currentTapTime <= tapDuration)
                    {
                        currentPhase = Phase.Tap;
                        _currentReleaseTime = context.time;
                        Debug.Log("MultiTapOrHold: Performed");

                    }
                    else
                    {
                        context.Canceled();
                        Debug.Log("MultiTapOrHold: Canceled");
                    }
                }

                break;
            case Phase.Performed:
                if (!context.ControlIsActuated(releasePoint))
                {
                    Debug.Log("MultiTapOrHold: Canceled");
                    context.Canceled();
                    Reset();
                }
                break;
        }
    }
    private enum Phase
    {
        None,
        Tap,
        WaitForRelease,
        Performed,
    }

    private Phase currentPhase;
    public void Reset()
    {
//        Debug.Log("canceled");
        currentPhase = Phase.None;
        _tapCounter = 0;
    }
}
