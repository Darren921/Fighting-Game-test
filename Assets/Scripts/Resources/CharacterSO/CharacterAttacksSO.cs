using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterAttacksSO", menuName = "Scriptable Objects/CharacterAttacksSO")]
    public class CharacterAttacksSo : ScriptableObject
    {
        public List<AttackData> Attacks;
        
        private void OnValidate()
        {
            // var resultsList = Enum.GetValues(typeof(InputReader.Attack)).Cast<InputReader.Attack>()
            //     .ToList();
            // foreach (var result in resultsList.Where(result =>
            //              !Attacks.Contains(new AttackData { _attackInputResult = result })))
            // {
            //     if (result.Type != InputReader.AttackType.None)
            //         Attacks.Add(new AttackData
            //         {
            //             _attackInputResult = result
            //         });
            // }
        }
    }
    [Serializable]
    public struct AttackData : IEquatable<AttackData>
    {
        public bool Equals(AttackData other)
        {
            return _attackInputResult.Equals(other._attackInputResult) && Tag == other.Tag && Damage.Equals(other.Damage) &&
                   Knockback.Equals(other.Knockback) && HitStun.Equals(other.HitStun) && BlockStun.Equals(other.BlockStun);
        }

        public override bool Equals(object obj)
        {
            return obj is AttackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_attackInputResult, (int)Tag, Damage, Knockback, HitStun, BlockStun);
        }

        public enum Tags
        {
            Low,
            Mid,
            High,
        }
        public enum States
        {
            Jumping, 
            Standing,
            Crouching,
        }

        public InputReader.Attack _attackInputResult;
        public Tags Tag;
        public float Damage;
        public float Knockback;
        public float HitStun;
        public float BlockStun;

}