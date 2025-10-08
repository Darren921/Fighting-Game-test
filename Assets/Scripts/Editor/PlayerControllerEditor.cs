using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;


[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public VisualTreeAsset visualTree;

    private Toggle dToggle;
    private Toggle vToggle;

    private List<Foldout> debugVars;
    private List<Foldout> vars;

    //private List<Toggle> sectionToggles;
    private List<VisualElement> Sections;
    private DropdownField dropdownField;
    private List<string> sectionNames = new List<string>();
// variables 
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        visualTree.CloneTree(root);
        var allElements = root.Q<VisualElement>();

        
        dToggle = root.Q<Toggle>("ToggleDebug");
        vToggle = root.Q<Toggle>("ToggleVariables");
        debugVars = allElements.Query<Foldout>().Where(element => element.name.Contains("Debug")).ToList();
        vars = allElements.Query<Foldout>().Where(element => element.name.Contains("Vars")).ToList();
        
        Sections = allElements.Query().Where(element => element.name.Contains("Section")).ToList();


     
        dropdownField = root.Q<DropdownField>("InfoShown");
        dropdownField.choices = sectionNames;
      
        GetSectionNames();
        UpdateDebugMode(dToggle.value);
        UpdateVariableMode(vToggle.value);
        
        
        dropdownField.RegisterValueChangedCallback(OnDropDownChange);
        dToggle.RegisterValueChangedCallback(evt => { UpdateDebugMode(evt.newValue); });
        vToggle.RegisterValueChangedCallback(evt => { UpdateVariableMode(evt.newValue); });
        return root;
    }

    private void OnDropDownChange(ChangeEvent<string> evt)
    {
        foreach (var section in Sections)
        {
            section.style.display = section.name == evt.newValue || evt.newValue == "All"
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }

    private void GetSectionNames()
    {
        foreach (var element in Sections.Where(element => !sectionNames.Contains(element.name)))
        {
            sectionNames.Add(element.name);
        }

        sectionNames.Insert(0, "None");
        sectionNames.Add("All");
    }

    private void UpdateDebugMode(bool isDebugMode)
    {
        foreach (var debugSection in debugVars)
        {
            debugSection.style.display = isDebugMode ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void UpdateVariableMode(bool isVariableMode)
    {
        foreach (var var in vars)
        {
            var.style.display = isVariableMode ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}