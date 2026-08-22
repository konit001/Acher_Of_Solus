using UnityEngine;

public class PlayerIdleState : State
{
    public AnimationClip anim;
    public playerControl input;
    public override void Enter()
    {
        animator.Play(anim.name);
    }
    public override void Do()
    {
        float bob = Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f;
        input.visualTransform.localPosition = new Vector3(0, bob * 0.1f, 0);
        
        if (rb.linearVelocity.sqrMagnitude > 0.01f && !input.isDashing)
        {
            IsComplete = true;
        }
    }
    public override void Exit() { }
}
