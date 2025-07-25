using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class InputReader : MonoBehaviour
{
    public enum MovementInputResult
    {
        None,
        Up,
        Down,
        Forward,
        Backward,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
    }
    public enum AttackInputResult
    {
        None,
        Light,
        LightLeft,
        LightRight,
        Medium,
        MediumLeft,
        MediumRight,
        Heavy,
        HeavyLeft,
        HeavyRight,
    }
    PlayerController player;
  public  MovementInputResult currentMoveInput { get; private set; }
  public  AttackInputResult currentAttackInput  { get; private set; }
  
  public Queue<bufferedInput<MovementInputResult>> movementBuffer = new ();
  public Queue<bufferedInput<AttackInputResult>> attackBuffer = new();

  private float bufferTime;
  
   [SerializeField] internal List<string> movementinputsVisual = new();
   [SerializeField] internal List<string> AttackinputsVisual = new ();

  

   private int bufferCap;

  public struct bufferedInput<T>
   {
       public T input;
       public int curFrame;

       public bufferedInput(T input, int curFrame)
       {
           this.input = input;
           this.curFrame = curFrame;
       }

     
   }

   public void AddInput<T>(T input,Queue<bufferedInput<T>> inputBuffer) where T : struct 
   {
       var frame = Time.frameCount;
       if (inputBuffer.Count < bufferCap)
       {
           inputBuffer.Enqueue(new bufferedInput<T>(input, frame));
       }
   }
    /*public void AddMovementInput(MovementInputResult result)
    {
      var frame = Time.frameCount;
      
      if(movementBuffer.Count < bufferCap)
        movementBuffer.Enqueue(new bufferedInput<MovementInputResult>(result, frame));
      
    }
    public void AddAttackInput(AttackInputResult result)
    {
        var frame = Time.frameCount;
        if(attackBuffer.Count < bufferCap)
            attackBuffer.Enqueue(new bufferedInput<AttackInputResult>(result, frame));
    }*/


    private void Awake()
    {
        player = GetComponent<PlayerController>();
        bufferTime = 4f;
        bufferCap = 10;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    
 

        private void Update()
        {
            CheckMovementInput();
            UpdateInputBuffers();
        }

        private void UpdateInputBuffers()
        {
            var  curFrame = Time.frameCount;
            if (movementBuffer.Count > 0 && curFrame - movementBuffer.Peek().curFrame > bufferTime )
            {
                movementBuffer.Dequeue();
            }

            if (attackBuffer.Count > 0 &&  curFrame - attackBuffer.Peek().curFrame > bufferTime)
            {
                attackBuffer.Dequeue();
            }
            currentMoveInput = movementBuffer.Count > 0 ? movementBuffer.Peek().input : MovementInputResult.None;
            currentAttackInput = attackBuffer.Count > 0 ? attackBuffer.Peek().input : AttackInputResult.None;

            movementinputsVisual.Clear();
            foreach (var input in movementBuffer)
            {
                movementinputsVisual.Add($"{input.input} (F{input.curFrame})");
           }
            //
           //  AttackinputsVisual.Clear();
           // foreach (var input in attackBuffer)
           //  {
           //       AttackinputsVisual.Add($"{input.input} (F{input.curFrame})");
           //  }
        }



        public void CheckMovementInput()
        {
            //checking the movement inputted 
            var  playerInput = new Vector2(player.playerMove.x, player.playerMove.y);
            var x = playerInput.x;
            var y = playerInput.y;
            var threshold = 0.5f;
            var newInput = MovementInputResult.None;

            //Converting all inputs to absolute, and comparing to threshold to determine if input is a corner input (controller) 
            if (Mathf.Abs(x) > threshold && Mathf.Abs(y) > threshold)
            {
              
                if (x > 0 && y > 0) newInput = MovementInputResult.UpRight;
                else if (x < 0 && y > 0) newInput = MovementInputResult.UpLeft;
                else if (x < 0 && y < 0) newInput = MovementInputResult.DownLeft;
                else newInput = MovementInputResult.DownRight;
            }
            //else find the correct standard input 
            else if (Mathf.Abs(x) > threshold)
            {
                //switch if the player is facing the opposite  direction 
                if (!player.Reversed)
                {
                    newInput = (x > 0)  ? MovementInputResult.Forward : MovementInputResult.Backward;

                }
                else
                {
                    newInput = (x > 0)  ? MovementInputResult.Backward : MovementInputResult.Forward;

                }

            }
            else if(Mathf.Abs(y) > threshold)
            {
                newInput = (y > 0) ? MovementInputResult.Up : MovementInputResult.Down;
            }
            //Input the new input detected 
            AddInput(newInput,movementBuffer);
        }
}

