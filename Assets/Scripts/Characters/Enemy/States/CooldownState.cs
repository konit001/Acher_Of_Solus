using UnityEngine;

public class CooldownState : State
{
    private Enemy enemyCore => core as Enemy;

    public override bool CanEnter()
        => enemyCore != null && enemyCore.combat != null
           && Time.time < enemyCore.combat.cooldownUntil; // ยังอยู่ในช่วงคูลดาวน์หลังยิงครบ burst

    public override void Enter()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero; // หยุดนิ่ง
    }

    public override void Do()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    public override void Exit() { }
}
