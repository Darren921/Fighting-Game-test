using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;



[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public VisualTreeAsset visualTree;
    
    private Toggle toggle;
    private Foldout fields; 
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        visualTree.CloneTree(root);
        var toggle = root.Q<Toggle>("toggle");
        fields = root.Q<Foldout>("Debug");
        Debug.Log(fields);
        return root;
    }
    
}
