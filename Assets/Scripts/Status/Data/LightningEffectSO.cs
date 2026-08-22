using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "StatusEffectSO/Element/LightningEffectSO")]
public class LightningEffectSO : StatusEffectSO
{
    [SerializeField, Range(0f, 1f)] private float paralyzeChance = 0.3f;
    [SerializeField] private float lockDuration = 1f; // how long movement/attack stays blocked once a proc hits

    public override void OnApply(GameObject target, ActiveStatusEffect instance) => TryProc(target, instance);
    public override void OnTick(GameObject target, ActiveStatusEffect instance) => TryProc(target, instance);
    public override void OnRemove(GameObject target, ActiveStatusEffect instance) => SetCanAct(target, true);

    // counts the lock down in real time, independent of tickInterval
    public override void OnEffectUpdate(GameObject target, ActiveStatusEffect instance, float deltaTime)
    {
        if (instance.lockTimer <= 0f) return;

        instance.lockTimer -= deltaTime;
        if (instance.lockTimer <= 0f)
        {
            instance.lockTimer = 0f;
            SetCanAct(target, true);
        }
    }

    // rolled once per tickInterval; skipped while still locked from an earlier proc
    private void TryProc(GameObject target, ActiveStatusEffect instance)
    {
        if (instance.lockTimer > 0f) return;

        if (Random.value < paralyzeChance)
        {
            instance.lockTimer = lockDuration;
            SetCanAct(target, false);
        }
    }

    // paralyze stops everyone outright, bosses included
    private void SetCanAct(GameObject target, bool canAct)
    {
        IMoveSpeedModifiable movable = target.GetComponent<IMoveSpeedModifiable>();
        if (movable != null) movable.MoveSpeedMultiplier = canAct ? 1f : 0f;

        IActionGate actionGate = target.GetComponent<IActionGate>();
        if (actionGate != null) actionGate.CanAct = canAct;
    }
}
