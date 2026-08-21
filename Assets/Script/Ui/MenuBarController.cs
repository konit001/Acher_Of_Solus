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

    private int currentIndex = -1;

    public bool IsAnyPageOpen => currentIndex != -1;

    private void Awake()
    {
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
    }

    private void Update()
    {
        if (uiInput.instance.PreviousInput) CyclePrev();
        if (uiInput.instance.NextInput) CycleNext();
    }

    public void OpenPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
            pages[i].page.SetActive(i == index);

        currentIndex = index;
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
