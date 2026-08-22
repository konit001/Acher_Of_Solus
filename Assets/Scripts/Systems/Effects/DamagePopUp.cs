using UnityEngine;
using TMPro;
using System.Collections;



public class DamagePopUp : MonoBehaviour
{
    [SerializeField] private static GameObject DamagePopUpPrefab;
    public TextMeshProUGUI text;
    private Color textColor;
    public float disappearTimer;
    public float moveSpeed;
    public float initialYSpeed = 3f; // ความแรงตอนเด้งขึ้น
    public float gravity = 5f;       // แรงโน้มถ่วงที่ดึงลง
    private float currentYSpeed;
    private float currentXSpeed;

    public static DamagePopUp Create(float damage, Vector3 position, Color color, bool isCrit = false)
    {
        GameObject DamagePopUpPrefab = Resources.Load<GameObject>("DamagePopUp");
        GameObject obj = Instantiate(DamagePopUpPrefab, position, Quaternion.identity);
        DamagePopUp popUp = obj.GetComponent<DamagePopUp>();
        popUp.Setup(damage, color, isCrit);
        return popUp;
    }
    public static DamagePopUp Create(float damage, Vector3 position)
    => Create(damage, position, Color.white, false);
    public void Setup(float damage, Color color, bool isCrit)
    {
        text.SetText(damage.ToString());

        // 1. กำหนดสีตามธาตุที่รับมาเป็นหลัก
        textColor = color;
        text.color = textColor;

        if (isCrit)
        {
            text.fontSize *= 1.5f;
            text.color = textColor * 1.2f;
        }

        currentXSpeed = Random.Range(-1.5f, 1.5f);
        currentYSpeed = initialYSpeed;
        StartCoroutine(PopUpDisappear());
        Debug.Log("ดาเมจ: " + damage + " | ติดคริไหม?: " + isCrit);
    }
    void Update()
    {
        currentYSpeed -= gravity * Time.deltaTime;
        transform.position += new Vector3(currentXSpeed, currentYSpeed) * Time.deltaTime;
    }

    public IEnumerator PopUpDisappear()
    {
        yield return new WaitForSeconds(disappearTimer);

        float fadeOut = 0f;
        float fadeDuration = 0.5f;
        while (fadeOut < fadeDuration)
        {
            fadeOut += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeOut / fadeDuration);
            textColor.a = alpha;
            text.color = textColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
