using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public CharacterAttacksSo characterAttacks;
    public float gravScale;

    // All character data is here, add and remove as needed 
    
    [Header ("Health")]
    public int health;
    [Header ("Misc")]
    public string characterName;
    [Header("Movement")]
    public int walkSpeed;
    public int runSpeed;
    public float jumpHeight;
    public int airDashCharges;
    public int jumpCharges;
    [Header("Combat")]
    public int damage;
    public float lightKnockback;
    public float medKnockback;
    public float heavyKnockback;

    private void OnValidate()
    {
         characterAttacks = Resources.FindObjectsOfTypeAll<CharacterAttacksSo>().ToList().Find(so =>   so.name.Contains(characterName));
    }
}