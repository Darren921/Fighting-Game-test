using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public float gravScale;

    // All character data is here, add and remove as needed 
    public int health;
    
    
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
    

}
