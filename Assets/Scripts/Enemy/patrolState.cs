using UnityEngine;

public abstract class patrolState : State
{
    public AnimationClip anim;
    protected Enemy enemy => core as Enemy;

    public override void Enter()
    {
        if (anim != null)
            animator.Play(anim.name);
    }
    public override void Do()
    {    }
    public override void Exit() { }
}
