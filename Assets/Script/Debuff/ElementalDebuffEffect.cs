using UnityEngine;

[CreateAssetMenu(fileName = "ElementalDebuffEffect", menuName = "BuffEffect/Debuff/Elemental")]
public class ElementalDebuffEffect : DebuffEffect
{
    [Header("Element")]
    public ElementType elementType;

    [Header("Tier Upgrade")]
    public ElementalDebuffEffect nextTier; // ตัวที่จะอัพเกรดไปเมื่อโดนธาตุเดียวกันซ้ำ (null = เทียร์สูงสุดแล้ว)

    public override void OnApply(Debuff instance, DebuffController owner)
    {
        Debuff sameElement = owner.activeDebuffs.Find(d =>
            d != instance &&
            d.debuffEffect is ElementalDebuffEffect elemental &&
            elemental.elementType == elementType);

        if (sameElement != null && sameElement.debuffEffect is ElementalDebuffEffect current && current.nextTier != null)
        {
            owner.activeDebuffs.Remove(sameElement);
            owner.activeDebuffs.Remove(instance);
            owner.ApplyDebuff(current.nextTier);
            return;
        }

        base.OnApply(instance, owner);
    }
}
