using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class ItemSlotUI : MonoBehaviour , IPointerClickHandler 
{
    public Image icon;
    public TextMeshProUGUI amountText;
    private BaseItemData currentItem;
    [HideInInspector]public GameObject Item;
    [HideInInspector]public int slotIndex;
    void Awake()
    {
        Clear();
    }
    public void SetItem(BaseItemData item, int amount)
    {
        currentItem = item;
        icon.color = Color.white;
        icon.sprite = item.itemImage;
        amountText.color = Color.black;
        amountText.text = amount.ToString();
    }

    public void Clear()
    {
        currentItem = null;
        icon.color = new Color(1, 1, 1, 0);
        icon.sprite = null;
        amountText.color = new Color(1, 1, 1, 0);
        amountText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;

        UiManager.instance.UseItem(currentItem);
    }


}
