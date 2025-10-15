using System;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
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

    private readonly Dictionary<( MovementInputResult, AttackInputResult), AttackInputResult>
        _attackMoveActions = new()
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

    private PlayerController _player;
    public MovementInputResult CurrentMoveInput { get; private set; }
    public AttackInputResult CurrentAttackInput { get; private set; }

    public AttackInputResult LastAttackInput { get; private set; }
    public int LastAttackInputFrame { get; private set; }


    private readonly List<BufferedInput<MovementInputResult>> _movementBuffer = new();
    private readonly List<BufferedInput<AttackInputResult>> _attackBuffer = new();

    private float _bufferTime;

    [SerializeField] internal List<string> movementInputsVisual = new();
    [SerializeField] internal List<string> attackInputsVisual = new();


    private int _bufferCap;

    private struct BufferedInput<T>
    {
        public readonly T Input;
        public readonly int CurFrame;


        public BufferedInput(T input, int curFrame)
        {
            Input = input;
            CurFrame = curFrame;
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
    private void AddMovementInput(MovementInputResult result)
    {
        if (_movementBuffer.Count >= _bufferCap)
            _movementBuffer.RemoveAt(0);

        _movementBuffer.Add(new BufferedInput<MovementInputResult>(result, Time.frameCount));
    }

    private void AddAttackInput(AttackInputResult result)
    {
        result = CheckForNormals(result, CurrentMoveInput);
        if (_attackBuffer.Count >= _bufferCap)
            _attackBuffer.RemoveAt(0);
        if (result != AttackInputResult.None)
        {
            LastAttackInput = result;
            LastAttackInputFrame = Time.frameCount;
        }

        _attackBuffer.Add(new BufferedInput<AttackInputResult>(result, Time.frameCount));
    }


    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _bufferTime = 5f;
        _bufferCap = 10;
        _player.PlayerAttackAction += AddAttackInput;
    }

    private void OnDestroy()
    {
        _player.PlayerAttackAction -= AddAttackInput;
    }

    private void Update()
    {
        CheckMovementInput();
        UpdateInputBuffers();
    }

    private void UpdateInputBuffers()
    {
        var curFrame = Time.frameCount;
        _movementBuffer.RemoveAll(i => curFrame - i.CurFrame > _bufferTime);
        _attackBuffer.RemoveAll(i => curFrame - i.CurFrame > _bufferTime);

        CurrentMoveInput = _movementBuffer.Count > 0 ? _movementBuffer[^1].Input : MovementInputResult.None;
        CurrentAttackInput = _attackBuffer.Count > 0 ? _attackBuffer[^1].Input : AttackInputResult.None;

        movementInputsVisual.Clear();
        foreach (var input in _movementBuffer)
        {
            movementInputsVisual.Add($"{input.Input} (F{input.CurFrame})");
        }

        attackInputsVisual.Clear();
        foreach (var input in _attackBuffer)
        {
            attackInputsVisual.Add($"{input.Input} (F{input.CurFrame})");
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
            [(1, 0)] = !_player.Reversed ? MovementInputResult.Forward : MovementInputResult.Backward,
            [(-1, 0)] = !_player.Reversed ? MovementInputResult.Backward : MovementInputResult.Forward,
            [(1, 1)] = MovementInputResult.UpRight,
            [(-1, 1)] = MovementInputResult.UpLeft,
            [(1, -1)] = MovementInputResult.DownRight,
            [(-1, -1)] = MovementInputResult.DownLeft
        };
//            print(playerInput);
        AddMovementInput(lookup[(_player.PlayerMove.x, _player.PlayerMove.y)]);
    }

    private AttackInputResult CheckForNormals(AttackInputResult attackInputResult, MovementInputResult movementInput)
    {
        return _attackMoveActions.GetValueOrDefault((movementInput, attackInputResult), attackInputResult);
    }

    public MovementInputResult GetValidMoveInput()
    {
        if (CurrentMoveInput != MovementInputResult.None) return CurrentMoveInput;
        var validInput = _movementBuffer.FindLast(i => i.Input != MovementInputResult.None);
        CurrentMoveInput = validInput.Input;
//      print(currentMoveInput);
        return validInput.Input;

    }
}