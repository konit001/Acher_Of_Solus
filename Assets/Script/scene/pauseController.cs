using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using System.Collections;
using Player.Input;
using UI.Input;

public class PauseController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("ลาก GameObject ที่เป็นตัวกรอบหน้าต่าง Pause ทั้งหมดมาใส่ช่องนี้")]
    [SerializeField] private GameObject pauseMenuPanel; 

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitToMenuButton;
    [SerializeField] private Button TestScenceButton;

    private bool isPaused = false;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
        if (inventoryButton != null) inventoryButton.onClick.AddListener(OnInventoryClicked);
        if (mapButton != null) mapButton.onClick.AddListener(OnMapClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (exitToMenuButton != null) exitToMenuButton.onClick.AddListener(OnExitToMenuClicked);
        if (TestScenceButton != null) TestScenceButton.onClick.AddListener(OnTestScnceClicked);
    }

    void Update()
    {
        if (uiInput.instance.EscapeInput)
        {
            // 1. เช็คก่อนว่าเปิด Inventory ค้างไว้หรือไม่
            if (UiPanelController.instance != null && UiPanelController.instance.IsInventoryOpen)
            {
                // ถ้า Inventory เปิดอยู่ ให้ปิดแค่ Inventory อย่างเดียว แล้วข้ามการทำงานส่วนอื่นไปเลย
                UiPanelController.instance.CloseAll();
                return; 
            }

            // 2. ถ้าไม่ได้เปิด Inventory อยู่ ให้จัดการเปิด/ปิด Pause ตามปกติ
            if (isPaused)
            {
                OnResumeClicked(); 
            }
            else
            {
                PauseGame(); 
            }
        }
    }

    private void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true); 
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void OnResumeClicked()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false); 
        Time.timeScale = 1f; 
        isPaused = false;
    }

    public void OnInventoryClicked()
    {
        Debug.Log("เปิดหน้า Inventory จาก Pause Menu");
        
        // 1. ปิดหน้าต่าง Pause Menu ก่อน
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        isPaused = false;

        // 2. สั่งเปิด Inventory 
        // (ฟังก์ชัน ToggleInventory ใน UiPanelController จะจัดการหยุดเวลาให้เอง)
        UiPanelController.instance.ToggleInventory();
    }

    public void OnMapClicked()
    {
        Debug.Log("เปิดหน้า Map");
    }

    public void OnSettingsClicked()
    {
        Debug.Log("เปิดหน้า Settings");
    }

    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f; 
        StartCoroutine(TransitionToMainMenu());
    }

    public void OnTestScnceClicked()
    {
        Time.timeScale = 1f; 
        StartCoroutine(TransitionToTest());
    }

    private IEnumerator TransitionToMainMenu()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.StartFadeOut();
            while (FadeManager.instance.IsFading)
            {
                yield return null;
            }
        }
        SceneManager.LoadScene("StartGame");
    }

    private IEnumerator TransitionToTest()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.StartFadeIn();
            while (FadeManager.instance.IsFading)
            {
                yield return null;
            }
        }
        SceneManager.LoadScene("SampleScene");
    }
}