using UnityEngine;

// สถานะเดียวของกระสุน: ยืดคลิปให้เล่นจบพอดีตอนกระสุนหมดอายุ
// คลิป 1 วิ กับ lifetime 3 วิ = เล่นช้าลง 3 เท่า เฟรมสุดท้ายจึงมาถึงวินาทีที่ 3 พอดี
public class BulletState : State
{
    public AnimationClip anim;

    public override void Enter()
    {
        // Core ของกระสุนคือ Projectile เสมอ แต่กันไว้เผื่อ State นี้ถูกเอาไปแปะกับ Core อื่น
        Projectile projectile = core as Projectile;
        float lifetime = projectile != null ? projectile.Lifetime : 0f;

        // ไม่รู้ lifetime หรือคลิปยาว 0 ก็เล่นความเร็วปกติ ดีกว่าหารด้วยศูนย์แล้วได้ NaN
        animator.speed = (lifetime > 0f && anim.length > 0f) ? anim.length / lifetime : 1f;

        // กระสุนถูกดึงกลับมาใช้ซ้ำจาก pool จึงต้องบังคับเล่นจากเฟรม 0 ทุกครั้ง
        animator.Play(anim.name, 0, 0f);
    }
}
