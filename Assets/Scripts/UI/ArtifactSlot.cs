using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArtifactSlot : MonoBehaviour, IPointerClickHandler
{
    public ArtifactType artifactType;
    public Image icon;
    private BaseItemData currentItem;

    public bool IsEmpty => currentItem == null;
    public BaseItemData GetCurrentItem() => currentItem;
    void Awake()
    {
        Clear();
    }

    public void SetItem(BaseItemData item, int amount)
    {
        currentItem = item;
        icon.color = Color.white;
        icon.sprite = item.itemImage;
    }

    public void Clear()
    {
        currentItem = null;
        icon.color = new Color(1, 1, 1, 0);
        icon.sprite = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty) return;

        if (eventData.button == PointerEventData.InputButton.Right && UiPanelController.instance.IsInventoryOpen == true)
        {
            UiManager.instance.UnEquipArtifact(this);
        }
    }
}