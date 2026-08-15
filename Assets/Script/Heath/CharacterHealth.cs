using System.Collections;
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
