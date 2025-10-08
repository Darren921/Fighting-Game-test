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
    public static event Action OnDeath;
    internal bool _hit;

    private void Awake()
    {
        _player = gameObject.GetComponentInParent<PlayerController>();
    }

    private void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
//        print(otherPlayer.IsActiveFrame);

        if (other.gameObject.CompareTag("HitBox") && otherPlayer.IsActiveFrame)
        {
            print("hit");
            var target = OnHit(_player, otherPlayer);
            if (target is not null && !target.HitStun && !_hit  )
            {
                bool isWalkingBack =
                    (target._playerStateManager.CurrentStateName == "PlayerWalkingState" ||
                     target._playerStateManager.CurrentStateName == "PlayerCrouchMoveState") &&
                    target.InputReader.CurrentMoveInput == InputReader.MovementInputResult.Backward;

                if (isWalkingBack)
                {
                    target._playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Blocking);
                    return;
                }
                _hit = true;
                target.GetComponent<PlayerStateManager>().SwitchState(PlayerStateManager.PlayerStateTypes.HitStun);
                target.PlayerHitDetection.TakeDamage(10);
            }
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _player.AtBorder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _player.AtBorder = false;
    }

    private PlayerController OnHit(PlayerController sender, PlayerController receiver)
    {
        //Check the players buffers for last attack frame  and decide the player hit
        var attackBufferSender = sender.GetComponentInParent<InputReader>();
        var attackBufferReceiver = receiver.GetComponentInParent<InputReader>();

        Debug.Log(attackBufferSender.LastAttackInput);
        Debug.Log(attackBufferReceiver.LastAttackInput);


        if (attackBufferSender.LastAttackInput != InputReader.AttackInputResult.None &&
            attackBufferReceiver.LastAttackInput != InputReader.AttackInputResult.None)
        {
            var result = attackBufferSender.LastAttackInputFrame < attackBufferReceiver.LastAttackInputFrame
                ? sender
                : receiver;
            print(result);
            return result;
        }

        if (attackBufferSender.LastAttackInput != InputReader.AttackInputResult.None &&
            attackBufferReceiver.LastAttackInput == InputReader.AttackInputResult.None)
        {
            return receiver;
        }

        if (attackBufferSender.LastAttackInput == InputReader.AttackInputResult.None &&
            attackBufferReceiver.LastAttackInput != InputReader.AttackInputResult.None)
        {
            return sender;
        }

        return null;
    }

    public void TakeDamage(float damage)
    {
        // deal damage and active death event to trigger end of game 

        _player.Health -= damage;
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