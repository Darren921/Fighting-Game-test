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
    bool isWalkingBack;

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
            isWalkingBack = (_player._playerStateManager.CurrentStateName == "PlayerWalkingState" || _player._playerStateManager.CurrentStateName == "PlayerCrouchMoveState") && _player.InputReader.CurrentMoveInput == InputReader.MovementInputResult.Backward;
            if (isWalkingBack)
            {
                print("walk");
                _player._playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Blocking);
            }
            else
            {
                _player._playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.HitStun);
            }
            
            _player.PlayerHitDetection.TakeDamage(10);

        }
    
        if (other.gameObject.CompareTag("Wall"))
        {
            _player.AtBorder = true;
        }
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
        
        _player.Health -=  isWalkingBack ? damage * 0.25f : damage;
        OnPlayerHit?.Invoke();
        //print(otherPlayer.name);
        //print(_player.name);
        otherPlayer.StartCoroutine(!_player.AtBorder
            ? otherPlayer.PlayerKnockBack.KnockBackOtherPlayer(_player)
            : otherPlayer.PlayerKnockBack.KnockBackThisPlayer(otherPlayer));

        if (_player.Health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    
}