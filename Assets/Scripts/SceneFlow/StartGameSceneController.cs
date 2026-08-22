using UnityEngine;
using UnityEngine.UI; // แก้ไขเป็น UnityEngine.UI
using UnityEngine.SceneManagement; // เพิ่มอันนี้เข้ามาสำหรับคำสั่งเปลี่ยนฉาก
using System.Collections; // เพิ่มอันนี้สำหรับ Coroutine

public class StartGameSceneController : MonoBehaviour
{
    // เปลี่ยนจาก GameObject เป็น Button 
    [SerializeField] private Button startButton;
    [SerializeField] private Button galleryButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        // ผูกฟังก์ชันเข้ากับปุ่ม (เช็ค null ไว้กันลืมใส่ปุ่มใน Inspector)
        if (startButton != null) startButton.onClick.AddListener(OnStartGameClicked);
        if (galleryButton != null) galleryButton.onClick.AddListener(OnGalleryClicked);
        if (settingButton != null) settingButton.onClick.AddListener(OnSettingClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
    }

    // ฟังก์ชันเมื่อกดปุ่ม Start
    public void OnStartGameClicked()
    {
        // เรียก Coroutine เพื่อทำ Fade จอมืด ก่อนเปลี่ยนฉาก
        StartCoroutine(TransitionToGameScene());
    }

    private IEnumerator TransitionToGameScene()
    {
        // 1. สั่ง FadeManager ให้เริ่มมืดลง
        if (FadeManager.instance != null)
        {
            FadeManager.instance.StartFadeOut();

            // 2. รอจนกว่า Fade จะทำงานเสร็จ
            while (FadeManager.instance.IsFading)
            {
                yield return null;
            }
        }

        SceneManager.LoadScene("1");
    }

    // ฟังก์ชันเมื่อกดปุ่ม Gallery
    public void OnGalleryClicked()
    {
        Debug.Log("เปิดหน้า Gallery");
        // เดี๋ยวค่อยมาใส่โค้ดเปิดปิดหน้าต่างแกลลอรี่
    }

    // ฟังก์ชันเมื่อกดปุ่ม Setting
    public void OnSettingClicked()
    {
        Debug.Log("เปิดหน้า Setting");
        // เดี๋ยวค่อยมาใส่โค้ดเปิดปิดหน้าต่างตั้งค่า
    }

    // ฟังก์ชันเมื่อกดปุ่ม Exit
    public void OnExitClicked()
    {
        Debug.Log("ออกจากการเล่นเกม");
        Application.Quit(); // คำสั่งนี้จะทำงานตอน Build ออกมาเล่นจริง (ตอนเทสใน Unity Editor จะไม่เห็นผล)
    }
}