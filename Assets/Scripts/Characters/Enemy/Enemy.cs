using UnityEngine;

// แทนที่ EnemyBase เดิม: mirror pattern เดียวกับ playerControl.cs
// คือ Core subclass เป็นคนเรียก setupInstances() เอง + selectState() + stateMacines.state.Do() เอง
public abstract class Enemy : Core, IMoveSpeedModifiable
{
    public enemyStatus enemy;

    [Header("Enemy Type")]
    public enemyType enemyType;

    [Header("Patrol Settings")]
    public float patrolDir = 1f;
    public bool isFacingRight = true;
    [HideInInspector] public float startPoint;
    public float moveSpeedMultiplier = 1f;

    float IMoveSpeedModifiable.MoveSpeedMultiplier
    {
        get => moveSpeedMultiplier;
        set => moveSpeedMultiplier = value;
    }

    bool IMoveSpeedModifiable.ResistsFullStop => enemy != null && enemy.isBoss;

    [Header("State Machine & Combat")]
    public enemyCombat combat;

    protected virtual void Awake()
    {
        // rb = GetComponent<Rigidbody2D>();
        startPoint = transform.position.x;
    }

    protected virtual void Start()
    {
        setupInstances();

        if (transform.localScale.x > 0)
        {
            isFacingRight = true;
            patrolDir = 1f;
        }
        else
        {
            isFacingRight = false;
            patrolDir = -1f;
        }

        selectState();
    }

    protected virtual void Update()
    {
        selectState();
        stateMacines.state.Do();
        // stateMacines.state.DoBranch();
    }

    // protected virtual void FixedUpdate()
    // {
    //     stateMacines.state.FixDoBranch();
    // }

    protected virtual void selectState()
    {
        // childStates เรียงตาม State.priority (มาก→น้อย) แล้วจาก Core.GetSortedChildStates()
        foreach (State s in childStates)
        {
            if (s.CanEnter())
            {
                stateMacines.set(s);
                return;
            }
        }
    }

    public virtual void flip()
    {
        patrolDir *= -1f;
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
    }

    protected abstract void OnDrawGizmosSelected();
}