using UnityEngine;
public abstract class State : MonoBehaviour
{
    // เช็คว่าสถานะนี้ทำงานเสร็จสิ้นหรือยัง
    public bool IsComplete { get; protected set; }
    protected float StartTime;
    protected float time => Time.time - StartTime;
    protected Animator animator => core.animator;
    protected Rigidbody2D rb => core.rb;

    // อ้างอิงกลับไปยัง Core หลัก
    protected Core core;

    public stateMacines macine;
    public State parent;

    // Property สำหรับดูว่า State ปัจจุบันของ Machine คืออะไร
    public State state => macine.state;

    [Header("Selection")]
    public int priority = 0; // ยิ่งมาก = ถูกเช็คก่อน/สำคัญกว่า ตั้งค่าใน Inspector ไม่ต้องพึ่งลำดับ GameObject ใน Hierarchy

    // เงื่อนไขว่า state นี้เข้าได้หรือไม่ตอนนี้ ใช้โดย selectState() ของ Core ที่สืบทอดต่อ (เช่น Enemy)
    public virtual bool CanEnter() => false;

    /// ฟังก์ชันอำนวยความสะดวกในการสั่งเปลี่ยน State
    protected void set(State newState, bool forceReset = false)
    {
        macine.set(newState, forceReset);
    }

    /// ฟังก์ชันรับค่า Core จากคลาส Core.setupInstances()
    public void SetCore(Core _core)
    {
        core = _core;
    }

    // public void DoBranch()
    // {
    //     Do();
    //     state?.DoBranch();
    // }

    // public void FixDoBranch()
    // {
    //     FixDo();
    //     state?.FixDoBranch();
    // }

    /// รีเซ็ตค่าพื้นฐานทุกครั้งก่อนเข้าสู่ State ใหม่
    public void initialise()
    {
        IsComplete = false;
        StartTime = Time.time;
    }


    // --- ฟังก์ชัน Lifecycle สำหรับให้คลาสลูกนำไปเขียนทับ
    public virtual void Enter() { }

    public virtual void Do() { }

    public virtual void FixDo() { }

    public virtual void Exit() { }
}