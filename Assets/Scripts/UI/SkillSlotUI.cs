using UnityEngine;
using UnityEngine.UI;
using TMPro;

// หนึ่งโหนดสกิลในแผง Upgrade (ฝั่งขวา) — วางบน SkillSlot.prefab
public class SkillSlotUI : MonoBehaviour
{
    public Image icon;               // ลูก "Fill" เดิม
    public TextMeshProUGUI lockText; // ลูก "Lock" เดิม ใช้โชว์ทั้งเลเวลที่ต้องใช้/ราคา/สถานะ
    public Button button;

    private ActiveSkillData skill;
    private int requiredLevel;
    private int weaponLevel;

    // เก็บ skill/requiredLevel/weaponLevel เป็น field แทนการส่งผ่านพารามิเตอร์ซ้ำไปมา
    // เพื่อให้ Refresh() เรียกซ้ำเองได้ (หลัง TryUnlock) โดยไม่ต้องรับ argument
    public void Bind(weaponsData.WeaponSkillUnlock unlock, int weaponLevel)
    {
        skill = unlock.skill;
        requiredLevel = unlock.requiredLevel;
        this.weaponLevel = weaponLevel;

        gameObject.SetActive(skill != null);
        if (skill != null) Refresh();
    }

    private void Refresh()
    {
        bool levelMet = weaponLevel >= requiredLevel;
        SkillTreeManager.SkillState state = SkillTreeManager.Instance.GetState(skill);
        bool unlocked = state != null && state.status == SkillNodeStatus.Unlocked;
        bool canBuyNow = !unlocked && levelMet && SkillTreeManager.Instance.CanUnlock(skill);

        if (icon != null) { icon.sprite = skill.icon; icon.color = unlocked ? Color.white : new Color(1, 1, 1, 0.4f); }
        if (lockText != null) lockText.text = unlocked ? "" : (levelMet ? skill.skillPointCost.ToString() : $"Lv.{requiredLevel}");

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnClick(unlocked, canBuyNow));
    }

    private void OnClick(bool unlocked, bool canBuyNow)
    {
        if (unlocked) SkillCaster.instance?.EquipSkill(skill);
        else if (canBuyNow) { SkillTreeManager.Instance.TryUnlock(skill); Refresh(); }
    }
}
