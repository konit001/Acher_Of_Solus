using UnityEngine;
using System;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class StatUIEntry
{
    public StatusType statusType;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI statusAmount;
}
public class StatusBox : MonoBehaviour, IPointerClickHandler
{
    [Header("--- Data Reference ---")]
    public playerStatus currentPlayerStats;

    [Space(10)]
    [Header("--- Status Box UI ---")]
    public GameObject statusBox;
    public List<StatUIEntry> statusUIList;

    private void Start()
    {
        UpdateTextStatusAmount();
    }
    public void UpdateTextStatusAmount()
    {
        if (currentPlayerStats == null)
        {
            return;
        }
        SetStatUI(StatusType.Health, currentPlayerStats.maxHealth);
        SetStatUI(StatusType.Stamina, currentPlayerStats.stamina);
        SetStatUI(StatusType.Attack, currentPlayerStats.baseAttack);
        SetStatUI(StatusType.ElementalBonus, currentPlayerStats.ElementalBonus);
        SetStatUI(StatusType.CritRate, currentPlayerStats.CritRate, "{0}%");
        SetStatUI(StatusType.CritDamage, currentPlayerStats.CritDamage, "{0}%");
        SetStatUI(StatusType.Defend, currentPlayerStats.Defend);
    }
    private void SetStatUI(StatusType type, float value, string format = "{0}")
    {
        var query = from ui in statusUIList
                    where ui.statusType == type
                    select ui;

        StatUIEntry entry = query.FirstOrDefault();
        if (entry != null && entry.statusAmount != null)
        {
            entry.statusAmount.text = string.Format(format, value.ToString("0.##"));
        }
    }

    private void OnEnable()
    {
        if (currentPlayerStats != null)
            currentPlayerStats.OnStatusUpdated += UpdateTextStatusAmount;
    }

    private void OnDisable()
    {
        if (currentPlayerStats != null)
            currentPlayerStats.OnStatusUpdated -= UpdateTextStatusAmount;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        bool isActive = statusBox.activeSelf;
        statusBox.SetActive(!isActive);
    }
}
