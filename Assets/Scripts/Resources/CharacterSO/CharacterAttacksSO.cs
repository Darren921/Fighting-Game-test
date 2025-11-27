using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CharacterAttacksSO", menuName = "Scriptable Objects/CharacterAttacksSO")]
    public class CharacterAttacksSo : ScriptableObject
    {
        public AttackData[] DefaultLightAttacks = new[]
        {
            new AttackData(new InputReader.Attack(InputReader.AttackType.Light)),
            new  AttackData(new InputReader.Attack(InputReader.AttackType.Light), AttackData.Tags.Low , AttackData.States.Crouching ),
            new AttackData(new InputReader.Attack(InputReader.AttackType.Light), AttackData.Tags.High , AttackData.States.Jumping)
        };
        public AttackData[] DefaultMedAttacks = new[]
        {
            new AttackData(new InputReader.Attack(InputReader.AttackType.Medium)),
            new  AttackData(new InputReader.Attack(InputReader.AttackType.Medium), AttackData.Tags.Low , AttackData.States.Crouching ),
            new AttackData(new InputReader.Attack(InputReader.AttackType.Medium), AttackData.Tags.High , AttackData.States.Jumping)
        };
        public AttackData[] DefaultHeavyAttacks = new[]
        {
            new AttackData(new InputReader.Attack(InputReader.AttackType.Heavy)),
            new  AttackData(new InputReader.Attack(InputReader.AttackType.Heavy), AttackData.Tags.Low , AttackData.States.Crouching ),
            new AttackData(new InputReader.Attack(InputReader.AttackType.Heavy), AttackData.Tags.High , AttackData.States.Jumping)
        };
        
    
        public List<AttackData> Attacks;

        public AttackData ReturnAttackData(InputReader.Attack attack, AttackData.States state)
        {
            var attackUsed = Attacks.Find( data => data.Attack.Move == attack.Move && data.Attack.Type == attack.Type && data.State == state) ;
            if (attackUsed.Equals(new AttackData()))
            {
                switch (attack.Type)
                {
                    case InputReader.AttackType.Light:
                        attackUsed = DefaultLightAttacks.FirstOrDefault((data => data.State == state));
                        break;
                    case InputReader.AttackType.Medium:
                        attackUsed = DefaultMedAttacks.FirstOrDefault((data => data.State == state));
                        break;
                    case InputReader.AttackType.Heavy:
                        attackUsed = DefaultHeavyAttacks.FirstOrDefault((data => data.State == state));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

        public AttackData( InputReader.Attack attack  , Tags tag = Tags.Mid, States state = States.Standing, float damage = 0, float knockback = 0, float hitStun = 0, float blockStun = 0 )
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