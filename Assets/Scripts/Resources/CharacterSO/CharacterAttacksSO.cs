using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CharacterAttacksSO", menuName = "Scriptable Objects/CharacterAttacksSO")]
    public class CharacterAttacksSo : ScriptableObject
    {
        public AttackData DefaultStandingAttack = new AttackData(new InputReader.Attack(InputReader.AttackType.None, InputReader.MovementInputResult.None), AttackData.Tags.Mid , AttackData.States.Standing , 0f,0f,0f,0f );
        public AttackData DefaultCrouchingAttack = new AttackData(new InputReader.Attack(InputReader.AttackType.None, InputReader.MovementInputResult.None), AttackData.Tags.Low , AttackData.States.Crouching , 0f,0f,0f,0f ); 
        public AttackData DefaultJunpingAttack = new AttackData(new InputReader.Attack(InputReader.AttackType.None, InputReader.MovementInputResult.None), AttackData.Tags.High , AttackData.States.Jumping , 0f,0f,0f,0f );

        public List<AttackData> Attacks;

        public AttackData ReturnAttackData(InputReader.Attack attack)
        {
            var attackUsed = Attacks.Find( data => data.Attack.Move == attack.Move && data.Attack.Type == attack.Type) ;
            if (attackUsed.Equals(new AttackData()))
            {
                attackUsed = DefaultStandingAttack;
            }
            return attackUsed;
        }
        
    }
    [Serializable]
    public struct AttackData : IEquatable<AttackData>
    {
        public bool Equals(AttackData other)
        {
            return Attack.Equals(other.Attack) && Tag == other.Tag && Damage.Equals(other.Damage) &&
                   Knockback.Equals(other.Knockback) && HitStun.Equals(other.HitStun) && BlockStun.Equals(other.BlockStun) && State.Equals(other.State);
        }

        public override bool Equals(object obj)
        {
            return obj is AttackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Attack, (int)Tag, Damage, Knockback, HitStun, BlockStun,State);
        }

        public AttackData( InputReader.Attack attack, Tags tag, States state, float damage, float knockback, float hitStun, float blockStun)
        {
            Attack = attack;
            Tag = tag;
            State = state;
            Damage = damage;
            Knockback = knockback;
            HitStun = hitStun;
            BlockStun = blockStun;
        }

        public enum Tags
        {
            Low,
            Mid,
            High,
        }
        public enum States
        {
            Standing,
            Jumping, 
            Crouching,
        }

        public InputReader.Attack Attack;
        public Tags Tag;
        public States State;
        public float Damage;
        public float Knockback;
        public float HitStun;
        public float BlockStun;

}