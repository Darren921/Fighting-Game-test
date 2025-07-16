using System.Globalization;
using UnityEditor;

[CustomEditor(typeof(PlayerStateManager))]
public class PlayerStateManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    
        var manager = (PlayerStateManager)target;
        var player = manager.GetComponent<PlayerController>();
    
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current State", manager.CurrentStateName, EditorStyles.boldLabel);

    }
}