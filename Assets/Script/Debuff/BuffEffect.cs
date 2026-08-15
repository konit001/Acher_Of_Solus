using UnityEngine;

// [CreateAssetMenu(fileName = "BuffEffect", menuName = "BuffEffect/BuffEffect")]
// ปิดเมนูสร้าง asset ตรงๆ ของ base ไว้ (มิเรอร์ DebuffEffect.cs) — content จริงควรเป็น
// ElementalBuffEffect หรือ GeneralBuffEffect เสมอ
public class BuffEffect : ScriptableObject
{
    [Header("Buff Information")]
    public string buffName;
    public string description;
    public Sprite icon;
    public BuffType buffType;

    [Header("Buff Effects")]
    public float buffAmount;
    public float buffAmountPerTick;
    public float buffDuration;
    public int buffStack;

    [Header("Stat Modifier")]
    public StatusType affectedStat;
    public float statModifierPercent;

    // Hook สำหรับ subclass ที่ต้องการพฤติกรรมพิเศษตอน apply (เช่น เช็คอัพเกรดเทียร์)
    // default = ฮีลเข้าเป้าหมายถ้ามีตั้งค่า buffAmount ไว้ (ค่าบวก = ฮีล)
    public virtual void OnApply(Buff instance, BuffController owner)
    {
        if (buffAmount > 0f)
        {
            owner.Health?.takeDamage(-buffAmount);
        }
    }
}
