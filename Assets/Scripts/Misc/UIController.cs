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

    public Vector2 lastInput;
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
        Debug.Log(ctx.phase);
        if (ctx.ReadValue<Vector2>() != Vector2.zero)
        {
            if (eventSystem.currentSelectedGameObject && LastselectedObject != eventSystem.currentSelectedGameObject)
            {
                LastselectedObject = eventSystem.currentSelectedGameObject;
            }
            lastInput = ctx.ReadValue<Vector2>();
        }
        else if (ctx.ReadValue<Vector2>() == Vector2.zero)
        {
            var Nullcheck = CheckForNextTarget();
            if(!Nullcheck && LastselectedObject);
            {
                print("Other target found");
                
               nextTarget = CheckForNextTarget();
               Debug.Log(nextTarget);
            }
          if(nextTarget)  eventSystem.SetSelectedGameObject(nextTarget);
         
        }
    }

    private GameObject CheckForNextTarget()
    {
        if (lastInput.y < 0) return LastselectedObject.GetComponent<Selectable>().navigation.selectOnDown.gameObject;
        if(lastInput.y > 0 ) return LastselectedObject.GetComponent<Selectable>().navigation.selectOnUp.gameObject;
        if(lastInput.x < 0) return LastselectedObject.GetComponent<Selectable>().navigation.selectOnLeft.gameObject;
        if(lastInput.x < 0 ) return LastselectedObject.GetComponent<Selectable>().navigation.selectOnRight.gameObject;
        return LastselectedObject;
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
