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
        Foward,
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
   [SerializeField] internal List<MovementInputResult> MovementinputsVisual = new List<MovementInputResult>();
   [SerializeField] internal List<AttackInputResult> AttackinputsVisual = new List<AttackInputResult>();




    public float ReturnCurrentFrame(float currentFrame)
    {
        return Time.frameCount - currentFrame;
    }
  
    public IEnumerator AddMovementInput(MovementInputResult result, float frameCount)
    {
        //updates current move input and removes past ones in 3 frames 
        if(currentMoveInput == result) yield break;
        currentMoveInput = result;
        MovementinputsVisual.Add(result);
        frameCount = ReturnCurrentFrame(frameCount);
        yield return new WaitUntil(() => Time.frameCount - frameCount > 3);
       MovementinputsVisual.Remove(result);
      
    }
    public IEnumerator AddAttackInput(AttackInputResult result, float frameCount)
    {
        //updates current attack input and removes past ones in 3 frames 

        if(currentAttackInput == result) yield break;
        currentAttackInput = result;
        AttackinputsVisual.Add(result);
      print(result.ToString());
        frameCount = ReturnCurrentFrame(frameCount);
        yield return new WaitUntil(() => Time.frameCount - frameCount > 3);
        AttackinputsVisual.Remove(result);    
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
            CheckAttackInput();
            CheckMovementInput();
        }

        private void CheckAttackInput()
        {
            //Adding None when inputs are not active 
            if(!player.IsAttacking && currentAttackInput != AttackInputResult.None) StartCoroutine(AddAttackInput(AttackInputResult.None, Time.frameCount));
        }

        private void CheckMovementInput()
        {
            //checking the movement inputted 
            var  playerInput = new Vector2(player.playerMove.x, player.playerMove.y);

            if (playerInput.magnitude < 0.1f)
            {
                StartCoroutine(AddMovementInput(MovementInputResult.None, Time.frameCount));
                return;
            }
            

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
                    newInput = (x > 0)  ? MovementInputResult.Foward : MovementInputResult.Backward;

                }
                else
                {
                    newInput = (x > 0)  ? MovementInputResult.Backward : MovementInputResult.Foward;

                }

            }
            else if(Mathf.Abs(y) > threshold)
            {
                newInput = (y > 0) ? MovementInputResult.Up : MovementInputResult.Down;
            }
            //Input the new input detected 
            StartCoroutine(AddMovementInput(newInput,Time.frameCount));
        }
}

