using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// ตัวแสดงผลช่อง Q/E — ใช้ตัวเดียวกันทั้งในแผง Upgrade ("[Q] Empty") และบน HUD จริง
public class SkillEquipSlotUI : MonoBehaviour
{
    public Key key;
    public Image icon;
    public GameObject emptyLabel; // กลุ่มข้อความ "[Q] Empty / Pick a skill"

    void OnEnable()
    {
        if (SkillCaster.instance != null) SkillCaster.instance.OnSlotChanged += OnSlotChanged;
        Refresh();
    }

    void OnDisable()
    {
        if (SkillCaster.instance != null) SkillCaster.instance.OnSlotChanged -= OnSlotChanged;
    }

    void OnSlotChanged(Key changedKey, ActiveSkillData skill)
    {
        if (changedKey == key) Refresh();
    }

    void Refresh()
    {
        ActiveSkillData skill = SkillCaster.instance != null ? SkillCaster.instance.GetEquippedSkill(key) : null;
        bool has = skill != null;

        if (icon != null) { icon.enabled = has; icon.sprite = has ? skill.icon : null; }
        if (emptyLabel != null) emptyLabel.SetActive(!has);
    }
}
