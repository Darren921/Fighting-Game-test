using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CustomEditor(typeof(CharacterSODataBase), true), CanEditMultipleObjects]
public class PlayerSOEditor : Editor
{
    public VisualTreeAsset mainVisualTree;
    public VisualTreeAsset LVisualTree;
    private ListView listView;
    private SerializedProperty listProperty;
    TemplateContainer List;


    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        mainVisualTree.CloneTree(root);
        listView = root.Q<ListView>("ListView");
        listProperty = serializedObject.FindProperty("characterSoList");
        if (listView == null) Debug.LogError("listview property is null");
        if (listProperty == null) Debug.LogError("list property is null");
        listView.bindingPath = listProperty.propertyPath;


        listView.makeItem = () =>
        {
            List = LVisualTree.Instantiate();
            return List;
        };

        listView.bindItem = BindItem;
        
        return root;
    }

    private void BindItem(VisualElement element, int index)
    {
        var elementProperty = listProperty.GetArrayElementAtIndex(index);
        var characterRef = element.Q<ObjectField>("CharacterSO");
        var propertiesContainer = element.Q<VisualElement>("CharacterProperties");

        if (characterRef == null) Debug.LogError("character ref is null");
        if (propertiesContainer == null) Debug.LogError("propertiesContainer is null");

        characterRef?.BindProperty(elementProperty);

        characterRef?.UnregisterCallback<ChangeEvent<Object>, VisualElement>(OnObjectChanged);
        characterRef?.RegisterCallback<ChangeEvent<Object>, VisualElement>(OnObjectChanged, propertiesContainer);

        UpdateListElements(propertiesContainer, elementProperty.objectReferenceValue as CharacterSO);
    }


    private void UpdateListElements(VisualElement element, CharacterSO characterSo)
    {
        element.Clear();
        var foldoutElement = List.Q<Foldout>("CharacterSheet");
        if (characterSo == null)
        {
            foldoutElement.text = "No Character";
        }
        else
        {
            var CharSO = new SerializedObject(characterSo);
            var iterator = CharSO.GetIterator();
            if (iterator.NextVisible(true))
            {
                while (iterator.NextVisible(false))
                {
                    if (iterator.propertyPath == "characterName") foldoutElement.text = iterator.stringValue == "" ? "empty":  foldoutElement.text = iterator.stringValue;
                    
                    
                    var field = new PropertyField(iterator.Copy());
                    element.Add(field);
                }

                element.Bind(CharSO);
                element.style.display = DisplayStyle.Flex;
            }
            else
            {
                element.style.display = DisplayStyle.None;
            }
        }
    }

    private void OnObjectChanged(ChangeEvent<Object> evt, VisualElement element)
    {
        var ncharacterSO = evt.newValue as CharacterSO;
        UpdateListElements(element, ncharacterSO);
    }
}