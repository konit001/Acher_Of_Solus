using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    [Header("References")]
    public PlayerAttack playerAttack; // อ้างอิงไปที่สคริปต์บนตัวแม่
    public SkillCaster skillCaster;   // ไว้ยิง Animation Event ตอนสกิลถึงจังหวะลงดาเมจ/กระสุน/บัฟ

    void Awake()
    {
        // ถ้าลืมลากใส่ มันจะหาคอมโพเนนต์จากตัวแม่ (Parent) ให้เองอัตโนมัติ
        if (playerAttack == null)
        {
            playerAttack = GetComponentInParent<PlayerAttack>();
        }
        if (skillCaster == null)
        {
            skillCaster = GetComponentInParent<SkillCaster>();
        }
    }

    // ฟังก์ชันนี้แหละที่ Animation Event จะมองเห็นและเรียกใช้
    public void CallSpearDamage()
    {
        if (playerAttack != null)
        {
            // สั่งให้ตัวแม่ทำดาเมจ
            playerAttack.TriggerSpearDamage();
        }
    }

    // มิเรอร์ CallSpearDamage — Animation Event ของคลิป castAnim จะเรียกกลางคลิป
    public void CallSkillImpact()
    {
        if (skillCaster != null)
        {
            skillCaster.OnCastImpact();
        }
    }
}