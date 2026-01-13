using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    Selectable[] selectables;
    GameObject LastselectedObject;
    GameObject nextTarget;

    InputSystemUIInputModule inputModule;
    [SerializeField] EventSystem eventSystem;
    public static UIController instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
            DontDestroyOnLoad(this);
            
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
     
    }

    private void SceneManagerOnactiveSceneChanged(Scene last , Scene nextScene )
    {
        nextTarget = null;
        LastselectedObject = null;
        eventSystem = EventSystem.current;
        inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        inputModule.move.action.performed += ActionOnperformed;   
        if (!LastselectedObject) LastselectedObject = eventSystem.firstSelectedGameObject;

    }


    void Start()
    {
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
            var nextUITarget = eventSystem.currentSelectedGameObject?.GetComponent<Selectable>().navigation.selectOnDown?.gameObject;
            if(!nextUITarget && LastselectedObject);
            {
               nextTarget = LastselectedObject.GetComponent<Selectable>().navigation.selectOnDown?.gameObject;
            }
            if(nextTarget) eventSystem.SetSelectedGameObject(nextTarget);
         
        }
    }
    public  void SelectObject(Selectable selectable)
    {
        print(selectable.name);
        instance.eventSystem.SetSelectedGameObject(selectable.gameObject);
    }

    public  void DeselectObject()
    {
        instance.eventSystem.SetSelectedGameObject(null);
    }
    
}
