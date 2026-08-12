using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MouseChange mouseChange;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CursorManager.instance != null)
            CursorManager.instance.SetMode(mouseChange);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CursorManager.instance != null)
            CursorManager.instance.SetMode(MouseChange.Normal);
    }
}