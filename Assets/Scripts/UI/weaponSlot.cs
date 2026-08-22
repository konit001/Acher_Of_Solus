using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Slot { Slot1, Slot2 }

public class weaponSlot : MonoBehaviour, IPointerClickHandler
{
    public Slot slot;
    public Image icon;
    private BaseItemData currentItem;
    public bool IsEmpty => currentItem == null;
    public BaseItemData GetCurrentItem() => currentItem;
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
            UiManager.instance.UnEquipWeapon(this.slot);
        }
    }

    // public void OnDrop(PointerEventData eventData)
    // {
    //     if (currentItem == null)
    //     {
    //         GameObject dropped = eventData.pointerDrag;
    //         if (dragableItem != null)
    //         {
    //             dragableItem.parentAfterDrag = transform;
    //         }
    //     }
    // }
}
