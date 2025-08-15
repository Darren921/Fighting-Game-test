using System;
using UnityEngine;

public class hitDetection : MonoBehaviour, IDamageable
{
    private PlayerController player;
    internal PlayerController otherPlayer;

    public static event Action OnDeath;
    private void Awake()
    {
        player = gameObject.GetComponentInParent<PlayerController>();
    }

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            otherPlayer = other.gameObject.GetComponentInParent<PlayerController>();
            var target = OnHit(player, other.gameObject.GetComponentInParent<PlayerController>());
            target.GetComponent<PlayerStateManager>().SwitchState(PlayerStateManager.PlayerStateTypes.HitStun);
            target.playerHitDetection.TakeDamage(10);
        }
    }

    private PlayerController OnHit(PlayerController sender, PlayerController receiver)
    {
        var attackBufferSender = sender.GetComponentInParent<InputReader>();
        var attackBufferReceiver = receiver.GetComponentInParent<InputReader>();

     //   print(attackBufferSender!= InputReader.AttackInputResult.None);
//        print(attackBufferReceiver != InputReader.AttackInputResult.None);

    //    print(attackBufferSender);
    //    print(attackBufferReceiver);
     
        if (attackBufferSender.LastValidAttackInput != InputReader.AttackInputResult.None &&
            attackBufferReceiver.LastValidAttackInput != InputReader.AttackInputResult.None)
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
        }
        return null;
    }
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        player.Health -= damage;

        if (player.Health <= 0)
        {
       //     OnDeath?.Invoke();
        }
    }
}
