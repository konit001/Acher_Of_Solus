using UnityEngine;

public class PlayerDashState : State
{
    public AnimationClip anim;
    public playerControl input;
    public override void Enter()
    {
        animator.Play(anim.name);
    }
    public override void Do()
    {
        if (input.isDashing)
        {
            IsComplete = true;
        }
    }
    public override void Exit() { }
}
