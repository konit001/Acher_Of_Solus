using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    
    [SerializeField] private Image _fadeOutImage;
    [Range(0.1f, 10f), SerializeField] private float _fadeOutSpeed = 5f;
    [Range(0.1f, 10f), SerializeField] private float _fadeInSpeed = 5f;

    public bool IsFading { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartFadeOut()
    {
        if (!IsFading)
        {
            StartCoroutine(FadeRoutine(1f, _fadeOutSpeed));
        }
    }

    public void StartFadeIn()
    {
        if (!IsFading)
        {
            StartCoroutine(FadeRoutine(0f, _fadeInSpeed));
        }
    }

    // Coroutine เดียวจัดการได้ทั้ง Fade In และ Fade Out
    private IEnumerator FadeRoutine(float targetAlpha, float speed)
    {
        IsFading = true;
        Color currentColor = _fadeOutImage.color;

        // วนลูปจนกว่าค่า Alpha ปัจจุบันจะเท่ากับเป้าหมาย
        while (!Mathf.Approximately(currentColor.a, targetAlpha))
        {
            // Mathf.MoveTowards จะช่วยปรับค่าอย่างนุ่มนวลและไม่ให้ค่าทะลุเป้าหมาย
            currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, speed * Time.deltaTime);
            _fadeOutImage.color = currentColor;
            
            yield return null; // รอให้ผ่านไป 1 เฟรมแล้วค่อยทำรอบต่อไป
        }

        // ตรวจสอบความแน่ใจว่าค่าตรงเป๊ะเมื่อจบการทำงาน
        currentColor.a = targetAlpha;
        _fadeOutImage.color = currentColor;
        
        IsFading = false;
    }
}