using UnityEditor;

[CustomEditor(typeof(PlayerStateManager))]
public class PlayerStateManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var manager = (PlayerStateManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current State", manager.CurrentStateName, EditorStyles.boldLabel);
    }
}