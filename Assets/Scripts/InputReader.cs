using System.Collections.Generic;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public enum InputResults
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

        Light,
        LightLeft,
        LightRight,
        Medium,
        MediumLeft,
        MediumRight,
        Heavy,
        HeavyLeft,
        HeavyRight,


        AirDashLeft,
        AirDashRight,
    }

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
    
    private Dictionary<( MovementInputResult, AttackInputResult), AttackInputResult>
        attackMoveActions = new()
        {
            {
                (MovementInputResult.None, AttackInputResult.Light), AttackInputResult.Light
            },
            {
                (MovementInputResult.Forward, AttackInputResult.Light), AttackInputResult.LightRight
            },
            {
                (MovementInputResult.Backward, AttackInputResult.Light), AttackInputResult.LightLeft
            },

            {
                (MovementInputResult.None, AttackInputResult.Medium),
                AttackInputResult.Medium
            },
            {
                (MovementInputResult.Forward, AttackInputResult.Medium),
                AttackInputResult.MediumRight
            },
            {
                (MovementInputResult.Backward, AttackInputResult.Medium),
                AttackInputResult.MediumLeft
            },

            {
                (MovementInputResult.None, AttackInputResult.Heavy),
                AttackInputResult.Heavy
            },
        };

    PlayerController player;
    public MovementInputResult currentMoveInput { get; private set; }
    public AttackInputResult currentAttackInput { get; private set; }
    public int currentAttackInputFrame { get; private set; }
    
    public Queue<bufferedInput<MovementInputResult>> movementBuffer = new();
    public Queue<bufferedInput<AttackInputResult>> attackBuffer = new();

    private float bufferTime;

    [SerializeField] internal List<string> movementinputsVisual = new();
    [SerializeField] internal List<string> AttackinputsVisual = new();


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

    /*public void AddInput<T>(T input,Queue<bufferedInput<T>> inputBuffer) where T : struct
    {
        var frame = Time.frameCount;
        if (inputBuffer.Count < bufferCap)
        {
            if(input.GetType() == typeof(MovementInputResult))
            {

            }
            inputBuffer.Enqueue(new bufferedInput<T>(input, frame));
        }
    }*/
    public void AddMovementInput(MovementInputResult result)
    {
        if (movementBuffer.Count >= bufferCap)
            movementBuffer.Dequeue();
        
        movementBuffer.Enqueue(new bufferedInput<MovementInputResult>(result,Time.frameCount));
    }

    public void AddAttackInput(AttackInputResult result)
    {
        result = checkForNormals(result, currentMoveInput);
        if (attackBuffer.Count >= bufferCap)
            movementBuffer.Dequeue();
        attackBuffer.Enqueue(new bufferedInput<AttackInputResult>(result, Time.frameCount));
    }


    private void Awake()
    {
        player = GetComponent<PlayerController>();
        bufferTime = 3f;
        bufferCap = 10;
        player.PlayerAttackAction += AddAttackInput;
    }

    private void Update()
    {
        CheckMovementInput();
        UpdateInputBuffers();
    }

    private void UpdateInputBuffers()
    {
        var curFrame = Time.frameCount;
        while (movementBuffer.Count > 0 && curFrame - movementBuffer.Peek().curFrame > bufferTime)
        {
            movementBuffer.Dequeue();
        }

        while (attackBuffer.Count > 0 && curFrame - attackBuffer.Peek().curFrame > bufferTime)
        {
            attackBuffer.Dequeue();
        }

        currentMoveInput = movementBuffer.Count > 0 ? movementBuffer.Peek().input : MovementInputResult.None;
        currentAttackInput = attackBuffer.Count > 0 ? attackBuffer.Peek().input : AttackInputResult.None;
        currentAttackInputFrame = attackBuffer.Count > 0 ? attackBuffer.Peek().curFrame : 0;

        movementinputsVisual.Clear();
        foreach (var input in movementBuffer)
        {
            movementinputsVisual.Add($"{input.input} (F{input.curFrame})");
        }

        AttackinputsVisual.Clear();
        foreach (var input in attackBuffer)
        {
            AttackinputsVisual.Add($"{input.input} (F{input.curFrame})");
        }
    }


    private void CheckMovementInput()
    {
        //checking the movement inputted 
        var lookup = new Dictionary<(float, float), MovementInputResult>
        {
            [(0, 0)] = MovementInputResult.None,
            [(0, 1)] = MovementInputResult.Up,
            [(0, -1)] = MovementInputResult.Down,
            [(1, 0)] = !player.Reversed ? MovementInputResult.Forward : MovementInputResult.Backward,
            [(-1, 0)] = !player.Reversed ? MovementInputResult.Backward : MovementInputResult.Forward,
            [(1, 1)] = MovementInputResult.UpRight,
            [(-1, 1)] = MovementInputResult.UpLeft,
            [(1, -1)] = MovementInputResult.DownRight,
            [(-1, -1)] = MovementInputResult.DownLeft
        };
//            print(playerInput);
        AddMovementInput(lookup[(player.PlayerMove.x, player.PlayerMove.y)]);
    }
    
    private AttackInputResult checkForNormals(AttackInputResult attackInputResult, MovementInputResult movementInput)
    {
        return attackMoveActions.GetValueOrDefault((movementInput, attackInputResult), attackInputResult);
    }
    
    public AttackInputResult UseAttackInput()
    {
        if (attackBuffer.Count == 0) return AttackInputResult.None;
        var input = attackBuffer.Dequeue().input;
        return input;
    }
}