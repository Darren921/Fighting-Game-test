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

    private float _tapCounter;
    private float _holdTime;
    internal bool Holding;
    private float _currentTime;


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
                    _tapCounter++;
                    context.Started();
                    context.SetTimeout(duration + 0.00001f); 
                    _currentTime = Time.time;
//                    Debug.Log("waited");
                }
                break;

            case InputActionPhase.Started:
                if (context.ControlIsActuated(pressPoint))
                {
                    _holdTime = Time.time - _currentTime;
//                    Debug.Log(holdTime);
                    _tapCounter++;
             //       Debug.Log("started");
                    if (_tapCounter >= multiTapCount && _holdTime < 0.189)
                    {
                        if (_holdTime > 0.185 && !Holding)
                        {
                            context.Canceled();
                        }
                        Holding = false;
                        context.Performed();
                    }
                    else if (_tapCounter >= multiTapCount && _holdTime >= 0.189)
                    {
                        Holding = true;
                        context.PerformedAndStayPerformed();
                    }
                }
                break;

            case InputActionPhase.Performed:
                Debug.Log("performed" + _holdTime + " seconds");

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
        _tapCounter = 0;
        _holdTime = 0;
    }
}
