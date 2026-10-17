using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    [SerializeField] private Image _fadeOutImage;
    [SerializeField] private GameObject _canvasObject;
    [Range(0.1f, 10f), SerializeField] private float _fadeOutSpeed = 5f;
    [Range(0.1f, 10f), SerializeField] private float _fadeInSpeed = 5f;

    public bool IsFading { get; private set; }

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        gameObject.MakePersistent();
        _canvasObject.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        IsFading = false;

        Color c = _fadeOutImage.color;
        c.a = 1f;
        _fadeOutImage.color = c;
        _canvasObject.SetActive(true);

        StartFadeIn();
    }

    // ===== เรียกอันเดียวจบ: fade out -> โหลดซีน -> fade in =====
    public void LoadSceneWithFade(string sceneName)
    {
        if (IsFading) return;
        StartCoroutine(FadeOutThenLoad(sceneName));
    }

    private IEnumerator FadeOutThenLoad(string sceneName)
    {
        yield return FadeRoutine(1f, _fadeOutSpeed);   // fade out จนดำ
        yield return SceneManager.LoadSceneAsync(sceneName);             // OnSceneLoaded จะ fade in ให้เอง
    }
    // ==========================================================

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

    private IEnumerator FadeRoutine(float targetAlpha, float speed)
    {
        _canvasObject.SetActive(true);
        IsFading = true;
        Color currentColor = _fadeOutImage.color;

        while (!Mathf.Approximately(currentColor.a, targetAlpha))
        {
            currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, speed * Time.unscaledDeltaTime);
            _fadeOutImage.color = currentColor;
            yield return null;
        }

        currentColor.a = targetAlpha;
        _fadeOutImage.color = currentColor;

        IsFading = false;
        _canvasObject.SetActive(targetAlpha != 0f);
    }
}