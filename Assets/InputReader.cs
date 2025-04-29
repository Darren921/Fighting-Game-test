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
    PlayerMovement playerMovement;
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
    Vector2 playerInput;
    
   [SerializeField] internal List<InputResult> inputsVisual = new List<InputResult>();
   PlayerController.playerState curState;

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

   

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    
 

        private void Update()
        {
            playerInput = new Vector2(player.playerMoveX, player.playerMoveY);

            if (playerInput.magnitude < 0.1f)
            {
                StartCoroutine(AddInput(InputResult.None, Time.frameCount));
                return;
            }

            float x = playerInput.x;
            float y = playerInput.y;

            float threshold = 0.5f;

            InputResult newInput;

            print(new Vector2(x, y));
            if (Mathf.Abs(x) > threshold && Mathf.Abs(y) > threshold)
            {
              
                if (x > 0 && y > 0) newInput = InputResult.UpRight;
                else if (x < 0 && y > 0) newInput = InputResult.UpLeft;
                else if (x < 0 && y < 0) newInput = InputResult.DownLeft;
                else newInput = InputResult.DownRight;
            }
            else if (Mathf.Abs(x) > Mathf.Abs(y))
            {
              
                newInput = (x > 0) ? InputResult.Right : InputResult.Left;
            }
            else
            {
                // Vertical
                newInput = (y > 0) ? InputResult.Up : InputResult.Down;
            }

            StartCoroutine(AddInput(newInput,Time.frameCount));
        }

    

    
  
}

