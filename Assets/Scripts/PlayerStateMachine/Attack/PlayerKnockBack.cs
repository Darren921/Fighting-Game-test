using System.Collections;
using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    private bool _isBeingKnockedBack;
    private const float KnockBackTime = 0.1f;
    private float _hitDirectionForce;

    public IEnumerator KnockBackOtherPlayer(PlayerController player)
    {
        //Use this to knock back the other player 
        _isBeingKnockedBack = true;

        var hitDir = ReturnHitDir(player.PlayerHitDetection.otherPlayer);
        _hitDirectionForce = ReturnHitForce(player.PlayerHitDetection.otherPlayer);
        var hitForce = hitDir * _hitDirectionForce;
        var elapsedTime = 0f;
        while (elapsedTime < KnockBackTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            player.rb.linearVelocity = hitForce;
            yield return new WaitForFixedUpdate();
        }

        _isBeingKnockedBack = false;
    }

    private Vector3 ReturnHitDir(PlayerController player)
    {
        // depending on the players direction return the direction 
        return !player.Reversed ? Vector3.right : Vector3.left;
    }

    public IEnumerator KnockBackThisPlayer(PlayerController player)
    {
        _isBeingKnockedBack = true;
        //Use this to knock back the attacking player 

        var hitDir = ReturnHitDir(player.PlayerHitDetection.otherPlayer);
        _hitDirectionForce = ReturnHitForce(player);
        var hitForce = hitDir * _hitDirectionForce;
        var elapsedTime = 0f;
        while (elapsedTime < KnockBackTime)
        {
            elapsedTime += Time.fixedDeltaTime;
            player.rb.linearVelocity = hitForce;
            yield return new WaitForFixedUpdate();
        }

        _isBeingKnockedBack = false;
    }


    private float ReturnHitForce(PlayerController player)
    {
        //Depending on the attack type return a knockback force value  (note mod this to add directional values later)
        var hitForceTemp = 0f;

        switch (player.InputReader.lastAttackInput)
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

        print(hitForceTemp);
        return hitForceTemp;
    }
}
