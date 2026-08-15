using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// ติดบน instance ของ Buff_Debuff_Icon_Canvas แต่ละอัน
// background = ไอคอนเต็มตลอด (ถูก tint มืด/desaturate เวลายังไม่เต็มเกจ)
// fill = ไอคอนสี Type=Filled ที่ fillAmount ไล่ตามความคืบหน้าของเกจ
public class BuffDebuffIconSlot : MonoBehaviour
{
    [Header("References")]
    public Image background;
    public Image fill;

    [Header("Reveal Settings")]
    public Color emptyTint = new Color(0.15f, 0.15f, 0.15f, 1f);
    public float effectPunchScale = 1.2f;
    public float effectDuration = 0.25f;

    private bool wasFull;
    private Coroutine effectRoutine;

    public void SetIcon(Sprite sprite)
    {
        if (background != null) background.sprite = sprite;
        if (fill != null) fill.sprite = sprite;
    }

    public void SetFill(float t01)
    {
        t01 = Mathf.Clamp01(t01);

        if (fill != null) fill.fillAmount = t01;
        if (background != null) background.color = Color.Lerp(emptyTint, Color.white, t01);

        bool isFull = t01 >= 1f;
        if (isFull && !wasFull)
        {
            PlayFullEffect();
        }
        wasFull = isFull;
    }

    private void PlayFullEffect()
    {
        if (effectRoutine != null) StopCoroutine(effectRoutine);
        effectRoutine = StartCoroutine(PunchScaleRoutine());
    }

    private IEnumerator PunchScaleRoutine()
    {
        Vector3 baseScale = Vector3.one;
        float half = effectDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(baseScale, baseScale * effectPunchScale, elapsed / half);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(baseScale * effectPunchScale, baseScale, elapsed / half);
            yield return null;
        }

        transform.localScale = baseScale;
    }
}
