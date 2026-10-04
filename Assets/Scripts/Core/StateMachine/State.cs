using UnityEngine;
public abstract class State : MonoBehaviour
{
    public bool IsComplete { get; protected set; }
    protected float StartTime;
    protected float time => Time.time - StartTime;
    protected Animator animator => core.animator;
    protected Rigidbody2D rb => core.rb;

    protected Core core;
    public stateMacines macine;
    public State parent;
    public State state => macine.state;

    [Header("Selection")]
    public int priority = 0;
    public virtual bool CanEnter() => false;
    protected void set(State newState, bool forceReset = false)
    {
        macine.set(newState, forceReset);
    }

    public void SetCore(Core _core)
    {
        core = _core;
    }

    /// รีเซ็ตค่าพื้นฐานทุกครั้งก่อนเข้าสู่ State ใหม่
    public void initialise()
    {
        IsComplete = false;
        StartTime = Time.time;
    }


    public virtual void Enter() { }
    public virtual void Do() { }
    public virtual void FixDo() { }
    public virtual void Exit() { }
}