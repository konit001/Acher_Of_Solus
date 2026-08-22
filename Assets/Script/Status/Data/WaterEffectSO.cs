using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "StatusEffectSO/Element/WaterEffectSO")]
public class WaterEffectSO : StatusEffectSO
{
    [SerializeField, Range(0f, 1f)] private float slowMultiplier = 0.2f;

    public override void OnApply(GameObject target, ActiveStatusEffect instance)
    {
        IMoveSpeedModifiable movable = target.GetComponent<IMoveSpeedModifiable>();
        if (movable == null) return;

        movable.MoveSpeedMultiplier = movable.ResistsFullStop ? slowMultiplier : 0f;
    }

    public override void OnRemove(GameObject target, ActiveStatusEffect instance)
    {
        IMoveSpeedModifiable movable = target.GetComponent<IMoveSpeedModifiable>();
        if (movable != null) movable.MoveSpeedMultiplier = 1f;
    }

    public override void OnTick(GameObject target, ActiveStatusEffect instance) { } // one-shot lock, nothing to repeat
}
