using UnityEngine;
using Player.Input;

public class PlayerStateMachine : Core
{
    public static PlayerStateMachine instance;
    [Header("State Machine")]
    public State IdleState;
    public State MoveState;
    public State DashState;
    public State SpearAttackState;
    public State GunAttackState;
    public State SkillCastState;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        setupInstances();
        stateMacines.set(IdleState);
    }

    // Update is called once per frame
    void Update()
    {
        selectState();
        stateMacines.state.Do();
    }

    private void selectState()
    {
        if (playerControl.instance.isCasting)
        {
            stateMacines.set(SkillCastState);
            return;
        }
        if (playerControl.instance.isAttacking)
        {
            stateMacines.set(SpearAttackState);
            return;
        }
        // ใน selectState() แทนที่บล็อก isShooting เดิม
        bool gunActive = stateMacines.state == GunAttackState && !GunAttackState.IsComplete;
        if (playerControl.instance.isShooting || gunActive)
        {
            stateMacines.set(GunAttackState);
            return;
        }

        // เช็คว่าตัวละครกำลังไถลอยู่ (มีควาามเร็ว) หรือ ผู้เล่นกำลังกดปุ่มเดินอยู่ (มี Input)
        bool hasVelocity = playerControl.instance.rb.linearVelocity.sqrMagnitude > 0.1f;
        bool hasMoveInput = userInput.instance.moveInput.sqrMagnitude > 0.01f;

        // ถ้าเข้าเงื่อนไขอย่างใดอย่างหนึ่ง ถือว่ากำลังขยับ
        bool isMoving = hasVelocity || hasMoveInput;

        if (!isMoving && !playerControl.instance.isDashing)
        {
            stateMacines.set(IdleState);
        }
        else if (isMoving && !playerControl.instance.isDashing)
        {
            stateMacines.set(MoveState);
        }
    }
}
