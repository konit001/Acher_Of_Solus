using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class DebuffHealth
{
    public elementDebuffType debuffType;
    public Color colorImage;
    public int priority; // higher wins when multiple debuffs are active at once
}

public class CharacterHealth : MonoBehaviour , IDamageable
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

    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHp > 0f;

    [Header("Debuff Settings")]
    public DebuffHealth[] debuffHealths;
    public bool isBurn;
    public bool isFreeze;
    public bool isPoison;
    public bool isParalyzed;
    

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

    public virtual void TakeDamage(float damage)
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

    public virtual void ChangeHpBar(elementDebuffType debuffType, bool isActive)
    {
        switch (debuffType)
        {
            case elementDebuffType.poison: isPoison = isActive; break;
            case elementDebuffType.Freeze: isFreeze = isActive; break;
            case elementDebuffType.Burn: isBurn = isActive; break;
            case elementDebuffType.Paralyzed: isParalyzed = isActive; break;
            default: return;
        }

        ApplyHighestPriorityColor();
    }

    // picks the active debuff with the highest configured priority (Inspector-driven, not hardcoded)
    private void ApplyHighestPriorityColor()
    {
        DebuffHealth best = null;

        foreach (DebuffHealth dh in debuffHealths)
        {
            if (!IsDebuffActive(dh.debuffType)) continue;
            if (best == null || dh.priority > best.priority)
                best = dh;
        }

        SetFillColor(best != null ? best.colorImage : normalColor);
    }

    private bool IsDebuffActive(elementDebuffType type)
    {
        switch (type)
        {
            case elementDebuffType.poison: return isPoison;
            case elementDebuffType.Freeze: return isFreeze;
            case elementDebuffType.Burn: return isBurn;
            case elementDebuffType.Paralyzed: return isParalyzed;
            default: return false;
        }
    }

    // keeps the bar visible even if a configured color's alpha was left at 0
    private void SetFillColor(Color color)
    {
        color.a = 1f;
        fillHp.color = color;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
