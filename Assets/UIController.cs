using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    Selectable[] selectables;
    GameObject LastselectedObject;
    GameObject nextTarget;

    InputSystemUIInputModule inputModule;
    [SerializeField] EventSystem eventSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
      eventSystem = EventSystem.current;
      inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
      inputModule.move.action.performed += ActionOnperformed; 
    }

    void Start()
    {
        LastselectedObject = eventSystem.currentSelectedGameObject;
        if (!LastselectedObject) LastselectedObject = eventSystem.firstSelectedGameObject;
        inputModule.deselectOnBackgroundClick = true;
    }

    private void ActionOnperformed(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>() != Vector2.zero)
        {
            if (eventSystem.currentSelectedGameObject && LastselectedObject != eventSystem.currentSelectedGameObject)
            {
                LastselectedObject = eventSystem.currentSelectedGameObject;
            }
        }
        else if (ctx.ReadValue<Vector2>() == Vector2.zero)
        {
            if(!eventSystem.currentSelectedGameObject?.GetComponent<Selectable>().navigation.selectOnDown.gameObject);
            {
                nextTarget = LastselectedObject.GetComponent<Selectable>().navigation.selectOnDown.gameObject;
            }
            if(nextTarget) eventSystem.SetSelectedGameObject(nextTarget);
         
        }
    }

    private void Update()
    {
        
    }
    public void SelectObject(Selectable selectable)
    {
        eventSystem.SetSelectedGameObject(selectable.gameObject);
    }

    public void DeselectObject()
    {
        eventSystem.SetSelectedGameObject(null);
    }
    
}
