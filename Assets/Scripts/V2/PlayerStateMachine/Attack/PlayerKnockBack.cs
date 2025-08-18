using System.Collections;
using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    private bool isBeingKnockedBack;
    private float _knockBackTime = 0.1f;
    private float _hitDirectionForce;

    public IEnumerator KnockBackOtherPlayer( PlayerController player)
    {
        isBeingKnockedBack = true;

        var  hitDir = ReturnHitDir(player.PlayerHitDetection.otherPlayer) ;
        _hitDirectionForce = ReturnHitForce(player.PlayerHitDetection.otherPlayer);
        var hitForce = hitDir * _hitDirectionForce ;
        var elapsedTime = 0f;
        while (elapsedTime < _knockBackTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            player.Rb.linearVelocity = hitForce;
            yield return new WaitForFixedUpdate();
        }
        isBeingKnockedBack = false;
    }
    
    private Vector3 ReturnHitDir(PlayerController player)
    {
        return player.Reversed ?  Vector3.left:  Vector3.right;
    }
    
    public IEnumerator KnockBackThisPlayer( PlayerController player)
    {
        isBeingKnockedBack = true;

        var  hitDir = ReturnHitDir(player.PlayerHitDetection.otherPlayer) ;
        _hitDirectionForce = ReturnHitForce(player);
        var hitForce = hitDir * _hitDirectionForce ;
        var elapsedTime = 0f;
        while (elapsedTime < _knockBackTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            player.Rb.linearVelocity = hitForce;
            yield return new WaitForFixedUpdate();
        }
        isBeingKnockedBack = false;
    }
    
    
    private float ReturnHitForce(PlayerController player)
    {
        var hitForceTemp = 0f;

        switch (player.InputReader.LastValidAttackInput)
        {
            case InputReader.AttackInputResult.Light:
                hitForceTemp = player.characterData.lightKnockback;
                break;
            case InputReader.AttackInputResult.LightLeft:
                hitForceTemp = player.characterData.lightKnockback;
                break;
            case InputReader.AttackInputResult.LightRight:
                hitForceTemp = player.characterData.lightKnockback;
                break;
            case InputReader.AttackInputResult.Medium:
                hitForceTemp = player.characterData.medKnockback;
                break;
            case InputReader.AttackInputResult.MediumLeft:
                hitForceTemp = player.characterData.medKnockback;
                break;
            case InputReader.AttackInputResult.MediumRight:
                hitForceTemp = player.characterData.medKnockback;
                break;
            case InputReader.AttackInputResult.Heavy:
                break;
            case InputReader.AttackInputResult.HeavyLeft:
                break;
            case InputReader.AttackInputResult.HeavyRight:
                break; 
        }
        print(player.characterData.lightKnockback);
        print(player.characterData.medKnockback);
        return hitForceTemp;
    }

    
}
