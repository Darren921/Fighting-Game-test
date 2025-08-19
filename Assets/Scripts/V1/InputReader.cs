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

    PlayerController player;
    public MovementInputResult currentMoveInput { get; private set; }
    public int currentMoveInputFrame { get; private set; }
    public AttackInputResult currentAttackInput { get; private set; }
    public int currentAttackInputFrame { get; private set; }

    public MovementInputResult LastValidMovementInput { get; private set; }
    public AttackInputResult LastValidAttackInput { get; private set; }
    public int  LastValidAttackInputFrame { get; private set; }


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
        var frame = Time.frameCount;

        if (result != MovementInputResult.None)
        {
            LastValidMovementInput = result;
        }

        if (movementBuffer.Count < bufferCap)
            movementBuffer.Enqueue(new bufferedInput<MovementInputResult>(result, frame));
    }

    public void AddAttackInput(AttackInputResult result)
    {
        var frame = Time.frameCount;
        if (result != AttackInputResult.None)
        {
            LastValidAttackInput = result;
            LastValidAttackInputFrame = frame;
        }
        if (attackBuffer.Count < bufferCap)
            attackBuffer.Enqueue(new bufferedInput<AttackInputResult>(result, frame));
    }


    private void Awake()
    {
        player = GetComponent<PlayerController>();
        bufferTime = 3f;
        bufferCap = 10;
    }

    private void Update()
    {
        CheckMovementInput();
        UpdateInputBuffers();
    }

    private void UpdateInputBuffers()
    {
        var curFrame = Time.frameCount;
        if (movementBuffer.Count > 0 && curFrame - movementBuffer.Peek().curFrame > bufferTime)
        {
            movementBuffer.Dequeue();
        }

        if (attackBuffer.Count > 0 && curFrame - attackBuffer.Peek().curFrame > bufferTime)
        {
            attackBuffer.Dequeue();
        }

        currentMoveInput = movementBuffer.Count > 0 ? movementBuffer.Peek().input : MovementInputResult.None;
        currentMoveInputFrame = movementBuffer.Count > 0 ? movementBuffer.Peek().curFrame : 0;
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
}