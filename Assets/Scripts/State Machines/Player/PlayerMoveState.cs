using UnityEngine;

public class PlayerMoveState : State
{
    public AnimationClip anim;
    public playerControl input;
    public override void Enter()
    {
        animator.Play(anim.name);
    }
    public override void Do()
    {
        if (rb.linearVelocity.x != 0 && !input.isDashing)
        {
            IsComplete = true;
        }
    }
    public override void Exit() { }
}
