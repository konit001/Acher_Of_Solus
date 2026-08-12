using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUi : MonoBehaviour
{
    public static LevelUi instance;
    public GameObject levelUiPanel;
    public Image levelFillPanel;
    public Image levelBgPanel;
    private int Exp;
    private int Level;
    public float fillSpeed = 5f;
    private Coroutine expCoroutine;

    public void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void Start()
    {
        levelFillPanel.fillAmount = 0;
        Exp = LevelSystem.instance.currentExp;
        Level = LevelSystem.instance.currentLevel;
    }

    public void Update()
    {
        if (Exp != LevelSystem.instance.currentExp || Level != LevelSystem.instance.currentLevel)
        {
            Exp = LevelSystem.instance.currentExp;
            if (expCoroutine != null) StopCoroutine(expCoroutine);
            expCoroutine = StartCoroutine(updateExp());
        }
    }

    IEnumerator updateExp()
    {

        float targetFill = (float)LevelSystem.instance.currentExp / LevelSystem.instance.expToNextLevel;
        while (Level < LevelSystem.instance.currentLevel)
        {
            while (levelFillPanel.fillAmount < 1f)
            {
                levelFillPanel.fillAmount = Mathf.MoveTowards(levelFillPanel.fillAmount, 1f, Time.deltaTime * (fillSpeed / 2f));
                yield return null;
            }
            levelFillPanel.fillAmount = 0f;
            Level++;
        }

        if (Level == LevelSystem.instance.currentLevel)
        {
            while (Mathf.Abs(levelFillPanel.fillAmount - targetFill) > 0.005f)
            {
                levelFillPanel.fillAmount = Mathf.Lerp(levelFillPanel.fillAmount, targetFill, Time.deltaTime * fillSpeed);
                yield return null;
            }
            levelFillPanel.fillAmount = targetFill;
        }
    }
}
