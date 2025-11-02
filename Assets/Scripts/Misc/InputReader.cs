using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
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
    public enum AttackType
    {
        None,
        Light,
        Medium,
        Heavy,
        Special,
    }
    
    [Serializable]
    public struct Attack : IEquatable<Attack>
    {
        public MovementInputResult Move;
        public AttackType Type;
        public Attack(AttackType type = AttackType.None ,MovementInputResult move = MovementInputResult.None)
        {
            Type = type;
            Move = move;
        }

        public override string ToString()
        {
            var move = Move != MovementInputResult.None ? $"{Move.ToString()} " : ""; 
            var fullMove = string.Concat(move, Type);
            return fullMove;
        }

        public bool Equals(Attack other)
        {
            return Move == other.Move && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is Attack other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Move, (int)Type);
        }
    }
    
    private PlayerController _player;
    public MovementInputResult CurrentMoveInput { get; private set; }
    public Attack CurrentAttackInput { get; private set; }
    public int CurrentAttackFrame { get; private set; }

    public Attack LastAttackInput { get; private set; }
    public int LastAttackInputFrame { get; private set; }

    public AttackData.States curState; 
    private readonly List<BufferedInput<MovementInputResult>> _movementBuffer = new();
    private readonly List<BufferedInput<Attack>> _attackBuffer = new();

    private int _bufferTime;

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

    private void AddAttackInput(AttackType type)
    {
        var Input = new Attack();
        Input = ReturnAttack(type, CurrentMoveInput);
        if (_attackBuffer.Count >= _bufferCap)
            _attackBuffer.RemoveAt(0);
        if (type != AttackType.None) 
        {
            LastAttackInput = Input;
            LastAttackInputFrame = Time.frameCount;
        }
        _attackBuffer.Add(new BufferedInput<Attack>(Input, Time.frameCount));

        
    }

    private AttackData.States CheckState(PlayerBaseState lastState)
    {
       // Debug.Log(lastState);
        var state = _player._playerStateManager.AirborneStates.Contains(lastState) ? AttackData.States.Jumping :
            _player._playerStateManager.StandingStates.Contains(lastState) ? AttackData.States.Standing :
            _player._playerStateManager.CrouchingStates.Contains(lastState) ? AttackData.States.Crouching : curState;
      //  Debug.Log(state.ToString());
        return state;

    }


    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _bufferTime = 5;
        _bufferCap = 10;
        _player.PlayerAttackAction += AddAttackInput;
    }

    private void OnDestroy()
    {
        _player.PlayerAttackAction -= AddAttackInput;
    }

    private void Update()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
            return;
        curState = CheckState(_player._playerStateManager.currentState);
        CheckMovementInput();
        UpdateInputBuffers();
    }


    private void UpdateInputBuffers()
    {
        var curFrame = Time.frameCount;
        _movementBuffer.RemoveAll(i => curFrame - i.CurFrame > _bufferTime);
        _attackBuffer.RemoveAll(i => curFrame - i.CurFrame > _bufferTime);

        CurrentMoveInput = _movementBuffer.Count > 0 ? _movementBuffer[^1].Input : MovementInputResult.None;
        CurrentAttackInput = _attackBuffer.Count > 0 ? _attackBuffer[^1].Input : new Attack();
        CurrentAttackFrame = _attackBuffer.Count > 0 ? _attackBuffer[^1].CurFrame : 0;

        if (curFrame - LastAttackInputFrame > _bufferTime && !_player.IsAttacking)
        {
            LastAttackInput = new Attack();
            LastAttackInputFrame = 0;
        }

        movementInputsVisual.Clear();
        foreach (var input in _movementBuffer)
        {
            movementInputsVisual.Add($"{input.Input} (F{input.CurFrame})");
        }

        attackInputsVisual.Clear();
        foreach (var input in _attackBuffer)
        {
            attackInputsVisual.Add($"{input.Input.ToString()} (F{input.CurFrame})");
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

    private Attack ReturnAttack(AttackType attackType, MovementInputResult movementInput)
    {
        return new Attack(attackType, movementInput);

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