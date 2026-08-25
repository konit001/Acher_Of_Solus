using UnityEngine;
using System;

[CreateAssetMenu(fileName = "playerStatus", menuName = "Status/playerStatus")]
public class playerStatus : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    [TextArea(3, 5)] public string description;
    public int level;
    public int exp;

    [Header("Base Stats")]
    public float maxHealth;
    public float stamina;

    [Header("Speed Stats")]
    public float acceleration;
    public float maxSpeed;
    public float deceleration;

    [Space(10)]
    [Header("Offensive Stats")]
    public float baseAttack;
    public float ElementalBonus;
    [Range(0f, 100f)] public float CritRate;
    public float CritDamage;
    [Tooltip("โอกาสทำให้เป้าหมายติดสถานะตามธาตุ (%)")]
    [Range(0f, 100f)] public float Luck;

    [Space(10)]
    [Header("Defensive Stats")]
    public float Defend;
    [Range(0f, 100f)] public float ResistanceDamage;

    [Space(10)]
    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    

    public event Action OnStatusUpdated;
    public void NotifyStatusChanged()
    {
        OnStatusUpdated?.Invoke();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            OnStatusUpdated?.Invoke();
        }
    }
}
