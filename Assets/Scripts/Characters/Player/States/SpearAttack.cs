using UnityEngine;

public class SpearAttack : State
{
    public AnimationClip anim;
    public PlayerAttack input;
    public override void Enter()
    {
        animator.Play(anim.name);
    }
    public override void Do()
    {
        // Debug.Log($"Current Time: {time} / Anim Length: {anim.length}");
        if (time >= anim.length)
        {
            IsComplete = true;
            Debug.Log("โจมตีเสร็จแล้ว! IsComplete เป็น True");
        }
    }
    public override void Exit()
    {

    }
}
