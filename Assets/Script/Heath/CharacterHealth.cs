using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [Header("HealthBar Image")]
    public Image fillHp;
    public Image trailHp;
    public Color normalColor = Color.red;

    [Header("Animation Settings")]
    protected float trailDelay = 0.5f;
    protected float trailSpeed = 1f;
    protected float currentHp;
    protected float maxHealth;
    protected Coroutine trailCoroutine;

    [Header("Debuff Settings")]
    public DebuffController debuffController;

    [Header("Buff Settings")]
    public BuffController buffController;

    [System.Serializable]
    public class DebuffColorEntry
    {
        public DebuffType type;
        public Color color = Color.white;
    }

    [Header("Debuff Color Override")]
    [SerializeField] private List<DebuffColorEntry> debuffColors = new List<DebuffColorEntry>();

    protected virtual void Awake()
    {
        debuffController = GetComponent<DebuffController>();
        buffController = GetComponent<BuffController>();
    }

    protected virtual void OnEnable()
    {
        if (debuffController != null) debuffController.OnDebuffsChanged += UpdateHealthBarColor;
    }

    protected virtual void OnDisable()
    {
        if (debuffController != null) debuffController.OnDebuffsChanged -= UpdateHealthBarColor;
    }

    protected virtual void Start()
    {
        fillHp.color = normalColor;
        currentHp = maxHealth;
    }

    protected virtual void Update()
    {
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public virtual void takeDamage(float damage)
    {
        currentHp -= damage;
        updateHealthBar();

        if (trailCoroutine != null)
        {
            StopCoroutine(trailCoroutine);
        }
        trailCoroutine = StartCoroutine(updateTrailHealthBar());
    }

    protected void updateHealthBar()
    {
        currentHp = Mathf.Clamp(currentHp, 0, maxHealth);
        if (fillHp != null) fillHp.fillAmount = currentHp / maxHealth;
    }

    // ไล่ debuffColors ตามลำดับที่ตั้งไว้ใน Inspector (ลำดับ = priority) ตัวแรกที่ active จะชนะ
    // ไม่มีตัวไหน active เลยก็กลับไปใช้ normalColor
    protected void UpdateHealthBarColor()
    {
        if (fillHp == null || debuffController == null) return;

        foreach (DebuffColorEntry entry in debuffColors)
        {
            if (debuffController.HasDebuffType(entry.type))
            {
                fillHp.color = entry.color;
                return;
            }
        }

        fillHp.color = normalColor;
    }

    protected IEnumerator updateTrailHealthBar()
    {
        yield return new WaitForSeconds(trailDelay);
        float targetRatio = currentHp / maxHealth;

        while (trailHp != null && trailHp.fillAmount > targetRatio)
        {
            trailHp.fillAmount = Mathf.MoveTowards(trailHp.fillAmount, targetRatio, trailSpeed * Time.deltaTime);
            yield return null;
        }

        if (trailHp != null) trailHp.fillAmount = targetRatio;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
