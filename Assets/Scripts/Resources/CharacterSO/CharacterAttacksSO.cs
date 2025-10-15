using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;


[System.Serializable]
public struct Attacks : IEquatable<Attacks>
{
    public enum Tags
    {
        Low,
        Medium,
        High,
    }

    public InputReader.AttackInputResult _attackInputResult;
    public Tags Tag;
    public float Damage;
    public float Knockback;
    public float HitStun;
    public float BlockStun;


    public bool Equals(Attacks other)
    {
        return _attackInputResult == other._attackInputResult && Tag == other.Tag && Damage.Equals(other.Damage) && Knockback.Equals(other.Knockback) && HitStun.Equals(other.HitStun) && BlockStun.Equals(other.BlockStun);
    }

    public override bool Equals(object obj)
    {
        return obj is Attacks other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)_attackInputResult, (int)Tag, Damage, Knockback, HitStun, BlockStun);
    }
}

[CreateAssetMenu(fileName = "CharacterAttacksSO", menuName = "Scriptable Objects/CharacterAttacksSO")]
public class CharacterAttacksSO : ScriptableObject
{
    public List<Attacks> Attacks;

    private void OnValidate()
    {
        var resultsList = Enum.GetValues(typeof(InputReader.AttackInputResult)).Cast<InputReader.AttackInputResult>()
            .ToList();
        foreach (var result in resultsList.Where(result => !Attacks.Contains(new Attacks { _attackInputResult = result })))
        {
            if (result != InputReader.AttackInputResult.None)
                Attacks.Add(new Attacks
                {
                    _attackInputResult = result
                });
        }
    }
}