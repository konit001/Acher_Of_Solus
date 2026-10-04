using UnityEngine;

// ล้อ SpearAttack/GunAttack แต่คลิปไม่ตายตัว เพราะสกิลแต่ละตัวมี castAnim ของตัวเอง
// SkillCaster ต้องเรียก SetClip() ก่อนที่ stateMacines.set() จะเรียก Enter() เสมอ
public class SkillCastState : State
{
    private AnimationClip clip;

    public void SetClip(AnimationClip newClip) => clip = newClip;

    public override void Enter()
    {
        if (clip == null) return;
        animator.Play(clip.name);
    }

    public override void Do()
    {
        if (clip == null) { IsComplete = true; return; }
        if (time >= clip.length) IsComplete = true;
    }

    public override void Exit() { }
}
