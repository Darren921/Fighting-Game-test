using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;

public class RangeProcessor : InputProcessor<Vector3>
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override Vector3 Process(Vector3 value, InputControl control)
    {
        
        for (var xyz = 0; xyz < 3; xyz++){
          value[xyz] = Mathf.RoundToInt(value[xyz]);
        }
        return value;
    }
#if UNITY_EDITOR
    static RangeProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterProcessor<RangeProcessor>();
    }
    
}
