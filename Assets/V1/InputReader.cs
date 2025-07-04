using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class InputReader : MonoBehaviour
{
    PlayerController player;
    public enum MovementInputResult
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

    MovementInputResult currentMoveInput = MovementInputResult.None;
    AttackInputResult currentAttackInput = AttackInputResult.None;
   [SerializeField] internal List<MovementInputResult> MovementinputsVisual = new List<MovementInputResult>();
   [SerializeField] internal List<AttackInputResult> AttackinputsVisual = new List<AttackInputResult>();




   public float ReturnCurrentFrame(float currentFrame)
    {
        return Time.frameCount - currentFrame;
    }
  
    public IEnumerator AddMovementInput(MovementInputResult result, float curframe)
    {
        if(currentMoveInput == result) yield break;
        currentMoveInput = result;
        curframe = ReturnCurrentFrame(curframe);
        MovementinputsVisual.Add(result);
       yield return  new WaitForSeconds(0.2f);
       MovementinputsVisual.Remove(result);
      
    }
    public IEnumerator AddAttackInput(AttackInputResult result, float frameCount)
    {
        if(currentAttackInput == result) yield break;
        currentAttackInput = result;
        AttackinputsVisual.Add(result);
//        print(result.ToString());
        frameCount = ReturnCurrentFrame(frameCount);
        yield return new WaitUntil(() => Time.frameCount - frameCount > 3);
        AttackinputsVisual.Remove(result);    }
    public MovementInputResult GetLastInput()
    {
      return currentMoveInput;
    }
    public  AttackInputResult GetLastAttackInput()
    {
        return currentAttackInput;
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
            if(!player.IsAttacking && currentAttackInput != AttackInputResult.None) StartCoroutine(AddAttackInput(AttackInputResult.None, Time.frameCount));
        }

        private void CheckMovementInput()
        {
            var  playerInput = new Vector2(player.playerMove.x, player.playerMove.y);

            if (playerInput.magnitude < 0.1f)
            {
                StartCoroutine(AddMovementInput(MovementInputResult.None, Time.frameCount));
                return;
            }
            

            float x = playerInput.x;
            float y = playerInput.y;
           
            if (player.Reversed)
            {
                x = -x;
            }

            float threshold = 0.5f;

            MovementInputResult newInput = MovementInputResult.None;

//            print(new Vector2(x, y));
            if (Mathf.Abs(x) > threshold && Mathf.Abs(y) > threshold)
            {
              
                if (x > 0 && y > 0) newInput = MovementInputResult.UpRight;
                else if (x < 0 && y > 0) newInput = MovementInputResult.UpLeft;
                else if (x < 0 && y < 0) newInput = MovementInputResult.DownLeft;
                else newInput = MovementInputResult.DownRight;
            }
            else if (Mathf.Abs(x) > threshold)
            {
              
                newInput = (x > 0) ? MovementInputResult.Right : MovementInputResult.Left;
            }
            else if(Mathf.Abs(y) > threshold)
            {
                // Vertical
                newInput = (y > 0) ? MovementInputResult.Up : MovementInputResult.Down;
            }

            StartCoroutine(AddMovementInput(newInput,Time.frameCount));
        }
}

