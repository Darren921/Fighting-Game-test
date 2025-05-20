using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "Scriptable Objects/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public int health;
    public int moveSpeed;
    public int jumpHeight;
    public int gravScale;
}
