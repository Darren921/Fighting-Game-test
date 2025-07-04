using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public int health;
    public int walkSpeed;
    public int runSpeed;
    public float jumpHeight;
    public float gravScale;
}
