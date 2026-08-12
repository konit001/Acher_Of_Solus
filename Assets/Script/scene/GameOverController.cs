using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System.Collections;

public class GameOverController : MonoBehaviour
{
    // สร้าง Instance เพื่อให้ PlayerHealth เรียกใช้ได้จากทุกที่
    public static GameOverController instance;

    [Header("UI Panels")]
    [Tooltip("ลากหน้าต่าง UI Game Over มาใส่ช่องนี้")]
    public GameObject gameOverPanel;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitToMenuButton;

    private void Awake()
    {
        // ผูก Instance เข้ากับตัวนี้
        if (instance == null) instance = this;
    }

    void Start()
    {
        // ซ่อนหน้า Game Over ไว้ก่อนตอนเริ่มเกม
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // ผูกปุ่มเข้ากับฟังก์ชันผ่านโค้ด
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
        if (exitToMenuButton != null) exitToMenuButton.onClick.AddListener(OnExitToMenuClicked);
    }

    // ฟังก์ชันนี้จะถูกเรียกจาก PlayerHealth ตอนที่ผู้เล่นตาย
    public void ShowGameOver()
    {
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true); // เปิดหน้าจอ Game Over
            Time.timeScale = 0f; 

            // (Optional) หากต้องการให้เกมหยุดนิ่งตอนตาย สามารถเอาคอมเมนต์ด้านล่างออกได้
            // Time.timeScale = 0f; 
        }
    }

    public void OnRestartClicked()
    {
        // คืนค่าเวลากลับเป็น 1 เสมอ 
        Time.timeScale = 1f; 

        // โหลดฉากปัจจุบันใหม่ 
        string currentSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(TransitionToScene(currentSceneName));
    }

    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f; 
        StartCoroutine(TransitionToScene("StartGame"));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.StartFadeOut();
            while (FadeManager.instance.IsFading)
            {
                yield return null;
            }
        }
        SceneManager.LoadScene(sceneName);
    }
}