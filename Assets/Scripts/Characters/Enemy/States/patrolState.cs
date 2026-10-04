using UnityEngine;

public abstract class patrolState : State
{
    public AnimationClip anim;
    protected Enemy enemy => core as Enemy;

    public override bool CanEnter() => true; // ไม่มีเงื่อนไข = fallback (ตั้ง priority ให้ต่ำสุดใน Inspector เช่น -999)

    public override void Enter()
    {
        if (anim != null)
            animator.Play(anim.name);
    }
    public override void Do()
    {    }
    public override void Exit() { }
}
