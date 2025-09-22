using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSODataBase", menuName = "Scriptable Objects/CharacterSODataBase")]
[System.Serializable]
public class CharacterSODataBase : ScriptableObject
{ 
    public  CharacterSO defaultCharacterSo; 
    public  List<CharacterSO> characterSo = new();   
    
    
    public  CharacterSO GetCharacterSoDataBase(int index)
    {
        if(index == 0) return defaultCharacterSo;
        var characterSoData = characterSo[index];
        Debug.Log(characterSoData.name);
        return characterSoData;
    }
}


