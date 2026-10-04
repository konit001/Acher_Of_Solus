using UnityEngine;
using UnityEngine.EventSystems;

public class HoverObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject icon;

    private void Start()
    {
        icon.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        icon.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        icon.SetActive(false);
    }
}
