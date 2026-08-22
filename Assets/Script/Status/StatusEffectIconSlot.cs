using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectIconSlot : MonoBehaviour
{
    public Image background;
    public Image fill;
    public TextMeshProUGUI stackText;

    public void SetIcon(Sprite sprite)
    {
        background.sprite = sprite;
        fill.sprite = sprite;
    }

    public void SetFill(float t01)
    {
        fill.fillAmount = Mathf.Clamp01(t01);
    }

    public void SetStack(int count)
    {
        if (stackText == null) return; // optional field, not every slot shows a stack count
        stackText.gameObject.SetActive(count > 1); // hide for single stacks
        stackText.text = count.ToString();
    }
}
