using UnityEngine;

[CreateAssetMenu(fileName = "ElementalBuffEffect", menuName = "BuffEffect/Buff/Elemental")]
public class ElementalBuffEffect : BuffEffect
{
    [Header("Element")]
    public ElementType elementType;

    [Header("Tier Upgrade")]
    public ElementalBuffEffect nextTier; // ตัวที่จะอัพเกรดไปเมื่อโดนธาตุเดียวกันซ้ำ (null = เทียร์สูงสุดแล้ว)

    public override void OnApply(Buff instance, BuffController owner)
    {
        Buff sameElement = owner.activeBuffs.Find(b =>
            b != instance &&
            b.buffEffect is ElementalBuffEffect elemental &&
            elemental.elementType == elementType);

        if (sameElement != null && sameElement.buffEffect is ElementalBuffEffect current && current.nextTier != null)
        {
            owner.activeBuffs.Remove(sameElement);
            owner.activeBuffs.Remove(instance);
            owner.ApplyBuff(current.nextTier);
            return;
        }

        base.OnApply(instance, owner);
    }
}
