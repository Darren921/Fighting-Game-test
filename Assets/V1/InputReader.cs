using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class InputReader : MonoBehaviour
{
    PlayerController player;
    public enum InputResult
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        Punch,
        Kick
        
    }
    InputResult[] directionMap = new InputResult[]
    {
        InputResult.Right,     
        InputResult.UpRight,   
        InputResult.Up,        
        InputResult.UpLeft,    
        InputResult.Left,      
        InputResult.DownLeft,  
        InputResult.Down,      
        InputResult.DownRight  
    };
   internal Vector2 playerInput;
    
   [SerializeField] internal List<InputResult> inputsVisual = new List<InputResult>();

   public float ReturnCurrentFrame(float currentFrame)
    {
        return Time.frameCount - currentFrame;
    }
  
    public IEnumerator AddInput(InputResult result, float curframe)
    {
        curframe = ReturnCurrentFrame(curframe);
        inputsVisual.Add(result);
       yield return  new WaitForSeconds(0.2f);
       inputsVisual.Remove(result);
      
    }

    public InputResult GetLastInput()
    {
        if (inputsVisual.Count == 0) return InputResult.None;
        return inputsVisual[^1];
    }


    private void Awake()
    {
        player = GetComponent<PlayerController>();
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    
 

        private void Update()
        {
            playerInput = new Vector2(player.playerMove.x, player.playerMove.y);

            if (playerInput.magnitude < 0.1f)
            {
                StartCoroutine(AddInput(InputResult.None, Time.frameCount));
                return;
            }

            float x = playerInput.x;
            float y = playerInput.y;

            float threshold = 0.5f;

            InputResult newInput = InputResult.None;

//            print(new Vector2(x, y));
            if (Mathf.Abs(x) > threshold && Mathf.Abs(y) > threshold)
            {
              
                if (x > 0 && y > 0) newInput = InputResult.UpRight;
                else if (x < 0 && y > 0) newInput = InputResult.UpLeft;
                else if (x < 0 && y < 0) newInput = InputResult.DownLeft;
                else newInput = InputResult.DownRight;
            }
            else if (Mathf.Abs(x) > threshold)
            {
              
                newInput = (x > 0) ? InputResult.Right : InputResult.Left;
            }
            else if(Mathf.Abs(y) > threshold)
            {
                // Vertical
                newInput = (y > 0) ? InputResult.Up : InputResult.Down;
            }

            StartCoroutine(AddInput(newInput,Time.frameCount));
        }

    

    
  
}

