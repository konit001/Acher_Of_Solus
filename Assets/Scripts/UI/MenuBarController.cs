using UnityEngine;
using UnityEngine.UI;
using UI.Input;

[System.Serializable]
public class MenuPage
{
    public Button button;
    public GameObject page;
}

public class MenuBarController : MonoBehaviour
{
    public static MenuBarController instance;

    [SerializeField] private MenuPage[] pages; // Inventory, Equipment, Skill, Map, Tasks, Codex
    [SerializeField] private MenuPage[] categoryBar;

    private int currentIndex = -1;   // หน้าหลักที่เปิดอยู่ (ใช้กับ CycleNext/CyclePrev)

    public bool IsAnyPageOpen => currentIndex != -1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"Duplicate MenuBarController on {gameObject.name}", this);
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        foreach (var p in pages) p.page.SetActive(false);
        for (int i = 0; i < pages.Length; i++)
        {
            int index = i; // capture for closure
            pages[i].button.onClick.AddListener(() => OpenPage(index));
        }

        // foreach (var c in categoryBar) c.page.SetActive(false);
        for (int i = 0; i < categoryBar.Length; i++)
        {
            if (categoryBar[i].button == null)
            {
                Debug.LogError($"categoryBar[{i}].button is not assigned", this);
                continue;
            }
            int index = i;
            categoryBar[i].button.onClick.AddListener(() => OpenCategory(index));
        }
        OpenCategory(0);   // ตั้งแท็บเริ่มต้นให้ฝั่งขวาสลับตามไปด้วยตั้งแต่แรก

    }

    private void Update()
    {
        if (UiPanelController.instance == null || !UiPanelController.instance.IsPanelOpen) return;
        if (uiInput.instance.PreviousInput) CyclePrev();
        if (uiInput.instance.NextInput) CycleNext();
    }

    public void OpenPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].page.SetActive(i == index);

        currentIndex = index;
        UiPanelController.instance?.SetCurrentPanel((UiPanelType)index);  // ← เพิ่ม
    }

    public void OpenCategory(int index)
    {
        for (int i = 0; i < categoryBar.Length; i++)
            categoryBar[i].page.SetActive(i == index);

        // แท็บย่อยที่เปิดอยู่ถูกจำไว้ที่ UiPanelController.CurrentCategory ที่เดียว
        // ตรงนี้จึงไม่แตะ currentIndex ของหน้าหลักอีกต่อไป (ของเดิมเขียนทับจน CycleNext/CyclePrev เพี้ยน)
        UiPanelController.instance?.SetCurrentCategory((EquipmentCategory)index);
    }

    public void CycleNext()
    {
        int next = IsAnyPageOpen ? (currentIndex + 1) % pages.Length : 0;
        OpenPage(next);
    }

    public void CyclePrev()
    {
        int prev = IsAnyPageOpen ? (currentIndex - 1 + pages.Length) % pages.Length : 0;
        OpenPage(prev);
    }

    public void CloseAll()
    {
        foreach (var p in pages) p.page.SetActive(false);
        currentIndex = -1;
    }
}
