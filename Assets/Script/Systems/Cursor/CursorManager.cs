using Unity.VisualScripting;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance { get; private set;}
    [SerializeField]private Texture2D CursorNormal;
    [SerializeField]private Texture2D CursorAtteck;
    private Vector2 CursorHotspot;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        CursorHotspot = new Vector2(5f , 5f);
        Cursor.SetCursor(CursorNormal , CursorHotspot , CursorMode.Auto);
    }
    public void SetMode(MouseChange mouseChange)
    {
        switch (mouseChange)
        {
            case MouseChange.Normal:
                CursorHotspot = new Vector2(5f , 5f);
                Cursor.SetCursor(CursorNormal , CursorHotspot , CursorMode.Auto);
            break;

            case MouseChange.Attack:
                CursorHotspot = new Vector2(CursorNormal.width / 2 , CursorNormal.height / 2);
                Cursor.SetCursor(CursorAtteck , CursorHotspot , CursorMode.Auto);
            break;

            default:
                CursorHotspot = new Vector2(5f , 5f);
                Cursor.SetCursor(CursorNormal , CursorHotspot , CursorMode.Auto);
            break;
        }
    }

    // Void OnDrawGizmoz(){}
}
