using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour, IDamageable
{
    private PlayerController _player;
    [SerializeField] internal PlayerController otherPlayer;
    public static event Action OnDeath;

    private void Awake()
    {
        _player = gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        //Search for other hit box and then apply affects 
        if (other.gameObject.CompareTag("HitBox"))
        {
            print("hit");
            var target = OnHit(_player, otherPlayer);

            if (target != null && !target.HitStun && target.Animator.GetBool(target.Active))
            {
                target.GetComponent<PlayerStateManager>().SwitchState(PlayerStateManager.PlayerStateTypes.HitStun);
                target.PlayerHitDetection.TakeDamage(10);
            }
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _player.AtBorder = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
      
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

    public void TakeDamage(int damage)
    {
        // deal damage and active death event to trigger end of game 

        _player.Health -= damage;
        //print(otherPlayer.name);
        //print(_player.name);
        StartCoroutine(FlashRed(_player)); //Red when hit


        otherPlayer.StartCoroutine(!_player.AtBorder
            ? otherPlayer.PlayerKnockBack.KnockBackOtherPlayer(_player)
            : otherPlayer.PlayerKnockBack.KnockBackThisPlayer(otherPlayer));

        if (_player.Health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    private IEnumerator FlashRed(PlayerController player) //bro wtf, this took me way to long to do
    {
        SkinnedMeshRenderer[] renderers = player.GetComponentsInChildren<SkinnedMeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        foreach (var render in renderers)
        {
            if (render != null)
            {
                render.GetPropertyBlock(block);
                block.SetColor("_BaseColor", Color.red);
                render.SetPropertyBlock(block);
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (var render in renderers)
        {
            if (render != null)
            {
                render.GetPropertyBlock(block);
                block.SetColor("_BaseColor", Color.white);
                render.SetPropertyBlock(block);
            }
        }
    }
}