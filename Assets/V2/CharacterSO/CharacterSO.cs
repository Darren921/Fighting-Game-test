using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    // All character data is here, add and remove as needed 
    public int health;
    public int walkSpeed;
    public int runSpeed;
    public int jumpHeight;
    public int gravScale;
}
