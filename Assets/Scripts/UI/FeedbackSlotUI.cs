using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// สคริปต์ของ FeedbackSlot.prefab — แถวบอกว่าเพิ่งเก็บอะไรเข้าตัวไปกี่ชิ้น
[RequireComponent(typeof(CanvasGroup))]
public class FeedbackSlotUI : MonoBehaviour
{
    public Image icon;                  // ลูกชื่อ "Image"
    public TextMeshProUGUI nameText;    // ลูกชื่อ "Name"
    public TextMeshProUGUI amountText;  // ลูกชื่อ "Amout"

    private CanvasGroup group;
    private Coroutine fadeCoroutine;
    private int amount;

    // ของชิ้นที่แถวนี้โชว์อยู่ — FeedbackListUI ใช้เช็คว่ารวมแถวเดิมได้ไหม
    public BaseItemData Item { get; private set; }

    void Awake() => group = GetComponent<CanvasGroup>();

    // เก็บของชิ้นใหม่ = เริ่มนับใหม่, ชิ้นเดิม = บวกทับของเดิม แล้วนับเวลาหายใหม่ทั้งคู่
    public void Show(BaseItemData item, float holdTime, float fadeTime)
    {
        amount = (Item == item) ? amount + 1 : 1;
        Item = item;

        icon.sprite = item.itemImage;
        icon.color = Color.white;
        nameText.text = item.itemName;
        amountText.text = "x" + amount;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        group.alpha = 1f;
        fadeCoroutine = StartCoroutine(HoldThenFade(holdTime, fadeTime));
    }

    // ซ่อนไม่ Destroy เพราะแถวถูกใช้ซ้ำ (แนวเดียวกับ MaterialListUI)
    public void Hide()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = null;
        Item = null;
        gameObject.SetActive(false);
    }

    // ใช้เวลาจริง เพราะเปิดแผงเมนูแล้ว UiPanelController ตั้ง Time.timeScale = 0 แถวจะค้างคาจอ
    private IEnumerator HoldThenFade(float holdTime, float fadeTime)
    {
        yield return new WaitForSecondsRealtime(holdTime);

        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }

        Hide();
    }
}
