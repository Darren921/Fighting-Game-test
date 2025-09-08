using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSODataBase", menuName = "Scriptable Objects/CharacterSODataBase")]
[System.Serializable]
public class CharacterSODataBase : ScriptableObject
{
    public  CharacterSO DefaultCharacterSO;
    public  List<CharacterSO> characterSO = new();   
    
    
    public  CharacterSO GetCharacterSODataBase(int index)
    {
        if(index == 0) return DefaultCharacterSO;
        var characterSOData = characterSO[index];
        Debug.Log(characterSOData.name);
        return characterSOData;
    }
}


