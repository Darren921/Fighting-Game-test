using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HitDetection : MonoBehaviour, IDamageable
{
    private PlayerController _player; 
    [SerializeField] internal PlayerController otherPlayer;
    public static event Action OnDeath;
    private void Awake()
    {
        _player = gameObject.GetComponentInParent<PlayerController>();
    }

    void Start()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        //Search for other hit box and then apply affects 
        if (other.gameObject.CompareTag("HitBox"))
        {
            var target = OnHit(_player, otherPlayer);
            target.GetComponent<PlayerStateManager>().SwitchState(PlayerStateManager.PlayerStateTypes.HitStun);
            target.PlayerHitDetection.TakeDamage(10);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _player.AtBorder = true;
        }
     
    }

    private PlayerController OnHit(PlayerController sender, PlayerController receiver)
    {
        
        /*//Check the players buffers for last attack frame  and decide the player hit
        var attackBufferSender = sender.GetComponentInParent<InputReader>();
        var attackBufferReceiver = receiver.GetComponentInParent<InputReader>();
        
        if (attackBufferSender.currentAttackInput != InputReader.AttackInputResult.None &&
            attackBufferReceiver.currentAttackInput != InputReader.AttackInputResult.None)
        {
            var result = attackBufferSender.LastValidAttackInputFrame < attackBufferReceiver.LastValidAttackInputFrame ?  sender : receiver ;
            print(result);
            return result;
        } 
        if (attackBufferSender.LastValidAttackInput != InputReader.AttackInputResult.None && attackBufferReceiver.LastValidAttackInput == InputReader.AttackInputResult.None)
        {
            return receiver;
        }
        if (attackBufferSender.LastValidAttackInput == InputReader.AttackInputResult.None && attackBufferReceiver.LastValidAttackInput != InputReader.AttackInputResult.None)
        {
            return sender;
        } */
        return null;
    }

    public void TakeDamage(int damage)
    {
        // deal damage and active death event to trigger end of game 
        
        _player.Health -= damage;
//        print(otherPlayer.name);
  //      print(_player.name);

        if (!_player.AtBorder)
        {
             otherPlayer.StartCoroutine(otherPlayer.PlayerKnockBack.KnockBackOtherPlayer(_player));
        }
        else
        {
            otherPlayer.StartCoroutine(otherPlayer.PlayerKnockBack.KnockBackThisPlayer(otherPlayer)); 
        }
        
        if (_player.Health <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}
