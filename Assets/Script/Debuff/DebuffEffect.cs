using UnityEngine;

// [CreateAssetMenu(fileName = "DebuffEffect", menuName = "BuffEffect/DebuffEffect")]
// ปิดเมนูสร้าง asset ตรงๆ ของ base ไว้ (เหมือน weaponsData.cs) — content จริงควรเป็น
// ElementalDebuffEffect หรือ GeneralDebuffEffect เสมอ
public class DebuffEffect : ScriptableObject
{
    [Header("Debuff Information")]
    public string debuffName;
    public string description;
    public Sprite icon;
    public DebuffType debuffType;

    [Header("Debuff Effects")]
    public float debuffDamage;
    public float debuffDamagePerTick;
    public float debuffDuration;
    public int debuffStack;

    [Header("Stat Modifier")]
    public StatusType affectedStat;
    public float statModifierPercent;

    [Header("Character Lock")]
    public bool disablesMovement;
    public bool disablesAction;

    // Hook สำหรับ subclass ที่ต้องการพฤติกรรมพิเศษตอน apply (เช่น เช็คอัพเกรดเทียร์)
    // default = ทำดาเมจ burst เข้าเป้าหมายถ้ามีตั้งค่า debuffDamage ไว้
    public virtual void OnApply(Debuff instance, DebuffController owner)
    {
        if (debuffDamage > 0f)
        {
            owner.Health?.takeDamage(debuffDamage);
        }
    }
}
