using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public abstract class SlotBase : MonoBehaviour, IPointerClickHandler
{
    protected BaseItemData currentItem;

    [Header("infomation")]
    public Image _Icon;
    public TextMeshProUGUI _Name;
    public TextMeshProUGUI _Level;
    public Image _LevelFill;
    public Button _Upgrade;
    [Tooltip("ลาก GameObject 'Material' ที่มี MaterialListUI ใส่ช่องนี้")]
    public MaterialListUI _Materials;

    public bool IsEmpty => currentItem == null;
    public BaseItemData GetCurrentItem() => currentItem;

    protected virtual void Awake()
    {
        // ช่องบน HUD ผูกแค่ไอคอนก็พอ แต่ถ้าไอคอนหายไปด้วยแปลว่าลืมผูกใน Inspector
        if (_Icon == null)
            Debug.LogWarning($"[{name}] ยังไม่ได้ผูก _Icon ใน Inspector ช่องนี้จะไม่แสดงไอคอน", this);
    }

    public virtual void SetItem(BaseItemData item, int amount)
    {
        currentItem = item;
        ShowInfo();
    }

    public virtual void Clear()
    {
        currentItem = null;
        HideInfo();
    }

    // แสดงข้อมูลของไอเทมในช่อง — ค่าเริ่มต้นโชว์แค่ไอคอนกับชื่อ
    // ส่วนเลเวล/อัปเกรดถูกซ่อนไว้ ให้ช่องที่ใช้จริง (weaponSlot) override เปิดเอง
    protected virtual void ShowInfo()
    {
        if (currentItem == null) return;

        if (_Icon != null)
        {
            _Icon.color = Color.white;
            _Icon.sprite = currentItem.itemImage;
        }

        if (_Name != null) _Name.text = currentItem.itemName;

        HideLevelInfo();
    }

    // ล้างข้อมูลทั้งหมดตอนช่องว่าง
    protected virtual void HideInfo()
    {
        if (_Icon != null)
        {
            _Icon.color = new Color(1, 1, 1, 0);
            _Icon.sprite = null;
        }

        if (_Name != null) _Name.text = "";

        HideLevelInfo();
    }

    // ซ่อนกลุ่มเลเวล + ปุ่มอัปเกรด + แถววัสดุทั้งหมด
    protected void HideLevelInfo()
    {
        if (_Level != null) _Level.text = "";
        if (_LevelFill != null) _LevelFill.fillAmount = 0f;

        if (_Upgrade != null)
        {
            _Upgrade.onClick.RemoveAllListeners();
            _Upgrade.interactable = false;
        }

        if (_Materials != null) _Materials.HideAll();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (UiPanelController.instance == null) return;
        if (!UiPanelController.instance.IsEquipmentOpen) return;

        OnRightClickInMenu();
    }

    protected abstract void OnRightClickInMenu();
}
