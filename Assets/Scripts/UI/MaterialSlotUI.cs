using UnityEngine;
using UnityEngine.UI;
using TMPro;

// สคริปต์ของ MeterialSlot.prefab — วาดวัสดุที่ใช้อัปเกรด 1 ชนิดต่อ 1 แถว
// โครงเดียวกับ StatusEffectIconSlot: เป็น view ล้วน ๆ รับคำสั่งผ่านเมธอด Set*
public class MaterialSlotUI : MonoBehaviour
{
    [Header("infomation")]
    public Image icon;                  // ไอคอนพื้นหลัง (บทบาทเดียวกับ background ของ StatusEffectIconSlot)
    public Image fill;                  // ไอคอนตัวเดียวกันแบบ Filled ค่อย ๆ เติมตามจำนวนที่เก็บได้

    public TextMeshProUGUI amountText;

    [Header("Colors")]
    public Color enoughColor = Color.white;
    public Color notEnoughColor = Color.red;

    public void SetIcon(Sprite sprite)
    {
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.color = sprite != null ? Color.white : new Color(1, 1, 1, 0);
        }

        if (fill != null) fill.sprite = sprite;
    }

    public void SetFill(float t01)
    {
        // icon กับ fill ยังผูกชี้ Image ตัวเดียวกันอยู่ fillAmount จึงมีผลเฉพาะตอนตั้ง Type = Filled
        if (fill == null || fill.type != Image.Type.Filled) return;

        fill.fillAmount = Mathf.Clamp01(t01);
    }

    // have = มีในกระเป๋ากี่ชิ้น, need = ต้องใช้กี่ชิ้น
    public void SetAmount(int have, int need)
    {
        if (amountText == null) return;

        amountText.text = have + "/" + need;
        amountText.color = have >= need ? enoughColor : notEnoughColor;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
