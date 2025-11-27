using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class HitDetection : MonoBehaviour, IDamageable
{
    private PlayerController _player;
    [SerializeField] internal PlayerController otherPlayer;
    
//    private PlayerController targetPlayer;
    private InputReader player1Input;
    public static event Action OnDeath;
    public static event Action OnPlayerHit;
    internal bool _hit;
    internal bool Blocking;

    private void Awake()
    {
        _player = gameObject.GetComponentInParent<PlayerController>();
        player1Input = _player.GetComponent<InputReader>();
    }

    private void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
//        print(otherPlayer.IsActiveFrame);

        if (other.gameObject.CompareTag("HitBox") && otherPlayer.IsActiveFrame && other.gameObject.activeInHierarchy && !otherPlayer.HitStun)
        {
            if (_hit) return;
            _hit = true;
            Blocking = CheckBlocking();
            print(Blocking);
            if (Blocking)
            {
                print("walk");
                _player._playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Blocking);
                _player.PlayerHitDetection.TakeDamage(otherPlayer.CharacterData.characterAttacks.ReturnAttackData(otherPlayer.InputReader.LastAttackInput,otherPlayer.InputReader.curState).Damage);
            }
            else
            {
                _player._playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.HitStun);
                _player.PlayerHitDetection.TakeDamage(otherPlayer.CharacterData.characterAttacks.ReturnAttackData(otherPlayer.InputReader.LastAttackInput,otherPlayer.InputReader.curState).Damage);

            }
            

        }
    
        if (other.gameObject.CompareTag("Wall"))
        {
            _player.AtBorder = true;
        }
    }

    private bool CheckBlocking()
    {
    //    print(_player._playerStateManager.CurrentStateName );
    //    print(_player.InputReader.CurrentMoveInput);
        
        if (_player._playerStateManager.currentState == _player._playerStateManager.States[PlayerStateManager.PlayerStateTypes.Walking] ||  _player._playerStateManager.currentState == _player._playerStateManager.States[PlayerStateManager.PlayerStateTypes.Crouching] || _player._playerStateManager.currentState ==  _player._playerStateManager.States[PlayerStateManager.PlayerStateTypes.Jumping] 
            && _player.InputReader.CurrentMoveInput is InputReader.MovementInputResult.Backward or InputReader.MovementInputResult.DownLeft or InputReader.MovementInputResult.UpLeft)
        {
            switch (_player.InputReader.curState)
            {
                case AttackData.States.Standing:
                    print("standing");
                    print(otherPlayer.InputReader.curState );
                    if (otherPlayer.InputReader.curState != AttackData.States.Crouching) return true;
                    break;
                case AttackData.States.Crouching:
                    print("Crouching");
                    print(otherPlayer.InputReader.curState );
                    if(otherPlayer.InputReader.curState != AttackData.States.Jumping) return true;
                    break;
                case AttackData.States.Jumping:
                    return true;
                default:
                    return false;
            }
            
        }
        print("Skippped");
        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        _player.AtBorder = false;
    }

    /*private PlayerController OnHit(PlayerController Player1, PlayerController Player2)
    {
        
        //Check the players buffers for last attack frame  and decide the player hit
     

//        Debug.Log(attackBufferSender.LastAttackInput);
//        Debug.Log(attackBufferReceiver.LastAttackInput);


        if (player1Input.LastAttackInput.Type!= InputReader.AttackType.None && player2Input.LastAttackInput.Type != InputReader.AttackType.None)
        {
            PlayerController result  = null;
            if (player1Input.LastAttackInputFrame < player2Input.LastAttackInputFrame )
            {
                result = Player1;
                Debug.Log($"Clash, {result} hit");
                return result;
            } 
            if (player2Input.LastAttackInputFrame < player1Input.LastAttackInputFrame )
            {
                result = Player2;
                Debug.Log($"Clash, {result} hit");
                return result;

            }
            return result;
        }

        if (player1Input.LastAttackInput.Type != InputReader.AttackType.None && player2Input.LastAttackInput.Type == InputReader.AttackType.None)
        {
            print("OtherPlayer NonContested ");
            return Player2;
        }

        if (player1Input.LastAttackInput.Type == InputReader.AttackType.None && player2Input.LastAttackInput.Type != InputReader.AttackType.None)
        {
            print("Player  NonContested ");

            return Player1;
        }

        return null;
    }*/

    public void TakeDamage(float damage)
    {
        // deal damage and active death event to trigger end of game 
        
        _player.Health -=  Blocking ? damage * 0.25f : damage;
//        print(_player.Health );
        OnPlayerHit?.Invoke();
        //print(otherPlayer.name);
        //print(_player.name);
        otherPlayer.StartCoroutine(!_player.AtBorder ? otherPlayer.PlayerKnockBack.KnockBackOtherPlayer(_player) : otherPlayer.PlayerKnockBack.KnockBackThisPlayer(otherPlayer));

        if (_player.Health <= 0)
        {
            _player.isDead = true;
            OnDeath?.Invoke();
        }
    }

    
}