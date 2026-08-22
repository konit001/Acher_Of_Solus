using System.Collections;
using Player.Input;
using Unity.Mathematics;
using UnityEngine;

public class playerControl : Core, IMoveSpeedModifiable
{
    [Header("Components & References")]
    public Transform visualTransform;
    public TrailRenderer trailRenderer;
    public Camera mainCamera;

    [Header("Player Stats & Movement")]
    public playerStatus playerStats;
    public float currentSpeed = 0f;
    public float moveSpeedMultiplier = 1f;

    float IMoveSpeedModifiable.MoveSpeedMultiplier
    {
        get => moveSpeedMultiplier;
        set => moveSpeedMultiplier = value;
    }

    bool IMoveSpeedModifiable.ResistsFullStop => true;

    [Header("Attack")]
    public PlayerAttack attackScript;

    [Header("State Machine")]
    public State IdleState;
    public State MoveState;
    public State DashState;
    public State SpearAttackState;
    public State GunAttackState;

    [Header("Status Flags")]
    public bool isFacingRight;
    public bool canDash { get; private set; }
    public bool isDashing { get; private set; }
    public bool isAttacking => attackScript != null && attackScript.isAttacking;
    public bool isShooting => attackScript != null && attackScript.isShooting;
    public Vector2 aimDirection { get; private set; } = Vector2.right;

    [Header("Look Settings")]
    public float maxTiltAngle = 25f;
    public float aimRotationSpeed = 8f;
    public float tiltRotationSpeed = 5f;
    public float shootAimHoldTime = 0.25f;

    private float currentVisualZ = 0f;
    private float shootHoldTimer = 0f;
    private bool wasShooting = false;
    private bool isAiming = false; // isAttacking || isShooting || ยังอยู่ในช่วง hold หลังยิง - ใช้จุดเดียวกันทั้งสคริปต์

    #region Unity Methods

    public void Start()
    {
        canDash = true;
        setupInstances();
        stateMacines.set(IdleState);

        // ตั้ง scale เริ่มต้นให้ตรงกับ isFacingRight ที่ตั้งไว้ใน inspector
        ApplyFacingScale();
    }

    private void Update()
    {
        UpdateAimState();

        selectState();
        stateMacines.state.Do();

        // ต้องหันตัวตามทิศเล็งก่อน แล้วค่อยคำนวณ tilt ทีหลัง
        // ไม่งั้น Rotate() จะใช้ isFacingRight ของเฟรมก่อนหน้าซึ่งอาจไม่ตรงกับ aimDirection ปัจจุบัน
        if (isAiming)
        {
            FaceAimDirection();
        }

        Rotate();

        if (isDashing) return;

        if (userInput.instance.sprintInput && canDash)
        {
            StartCoroutine(dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        move();
        if (!isAiming)
        {
            flip();
        }
    }

    #endregion

    #region State Logic

    private void selectState()
    {
        if (isAttacking)
        {
            stateMacines.set(SpearAttackState);
            return;
        }
        if (isShooting)
        {
            stateMacines.set(GunAttackState);
            return;
        }

        // เช็คว่าตัวละครกำลังไถลอยู่ (มีควาามเร็ว) หรือ ผู้เล่นกำลังกดปุ่มเดินอยู่ (มี Input)
        bool hasVelocity = rb.linearVelocity.sqrMagnitude > 0.1f;
        bool hasMoveInput = userInput.instance.moveInput.sqrMagnitude > 0.01f;

        // ถ้าเข้าเงื่อนไขอย่างใดอย่างหนึ่ง ถือว่ากำลังขยับ
        bool isMoving = hasVelocity || hasMoveInput;

        if (!isMoving && !isDashing)
        {
            stateMacines.set(IdleState);
        }
        else if (isMoving && !isDashing)
        {
            stateMacines.set(MoveState);
        }
    }

    #endregion

    #region Movement & Look Logic

    public void move()
    {
        Vector2 inputDir = userInput.instance.moveInput.normalized;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector2 targetVelocity = inputDir * playerStats.maxSpeed * moveSpeedMultiplier;
            float t = 1f - Mathf.Exp(-playerStats.acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, t);
            rb.linearDamping = 0f;
        }
        else
        {
            rb.linearDamping = playerStats.deceleration;
        }

        currentSpeed = rb.linearVelocity.magnitude;
    }

    public void flip()
    {
        float x = userInput.instance.moveInput.x;
        if (Mathf.Abs(x) <= 0.1f) return;

        SetFacing(x > 0);
    }

    public void FaceAimDirection()
    {
        if (Mathf.Abs(aimDirection.x) <= 0.01f) return; // เมาส์อยู่แนวตั้งพอดี ไม่ต้องหัน

        SetFacing(aimDirection.x > 0f);
    }

    // รวม flip() เดิมกับ SetFacing() เดิมไว้ที่เดียว ใช้ localScale.x ในการพลิกตัว (instant flip)
    // attackPoint เป็นลูกของ visualTransform อยู่แล้ว จึงพลิกตามอัตโนมัติ ไม่ต้องจัดการตำแหน่งแยก
    public void SetFacing(bool faceRight)
    {
        if (faceRight == isFacingRight) return;

        isFacingRight = faceRight;
        ApplyFacingScale();

        currentVisualZ = -currentVisualZ;
    }

    private void ApplyFacingScale()
    {
        if (visualTransform == null) return;

        Vector3 scale = visualTransform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1f : -1f);
        visualTransform.localScale = scale;
    }

    // เดิมคือ Z-tilt part ของ updateVisualTilt() - ส่วน yaw/snap logic ถูกตัดออกทั้งหมด
    // เพราะการหันตัวตอนนี้ใช้ localScale flip แทน ไม่ต้องหมุน yaw อีกต่อไป
    private void Rotate()
    {
        if (visualTransform == null) return;

        float targetZ;

        if (isAiming)
        {
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            targetZ = isFacingRight ? aimAngle : aimAngle - 180f;
        }
        else
        {
            float speedRatio = Mathf.Clamp01(rb.linearVelocity.magnitude / playerStats.maxSpeed);
            float verticalInput = rb.linearVelocity.y;

            // นำทิศทางการหันหน้ามาคูณ เพื่อกลับองศาตอนหันซ้าย
            float faceDirectionMultiplier = isFacingRight ? 1f : -1f;
            targetZ = Mathf.Clamp(verticalInput, -1f, 1f) * maxTiltAngle * speedRatio * faceDirectionMultiplier;
        }

        float rotSpeed = isAiming ? aimRotationSpeed : tiltRotationSpeed;
        currentVisualZ = Mathf.LerpAngle(currentVisualZ, targetZ, rotSpeed * Time.deltaTime);

        visualTransform.localRotation = Quaternion.Euler(0f, 0f, currentVisualZ);
    }

    // รวม updateAimDirection() เดิมกับ UpdateAimState() เดิมไว้ที่เดียว
    // เรียกครั้งเดียวต้นเฟรม จัดการทั้ง isAiming/shootHoldTimer และคำนวณ aimDirection จากเมาส์
    // (ข้าม aimDirection ระหว่างที่ isAiming อยู่ เหมือนเดิม)
    private void UpdateAimState()
    {
        if (wasShooting && !isShooting)
        {
            shootHoldTimer = shootAimHoldTime;
        }
        wasShooting = isShooting;

        if (shootHoldTimer > 0f)
        {
            shootHoldTimer -= Time.deltaTime;
        }

        isAiming = isAttacking || isShooting || shootHoldTimer > 0f;

        if (isAiming || mainCamera == null) return;

        Vector3 mouseScreenPos = userInput.instance.lookInput;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (Vector2)(mouseWorldPos - transform.position);
        if (dir.sqrMagnitude > 0.0001f)
        {
            aimDirection = dir.normalized;
        }
    }

    #endregion

    #region Coroutines

    public IEnumerator dash()
    {
        canDash = false;
        isDashing = true;

        float originGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        Vector2 dashVelocity;
        if (userInput.instance.moveInput != Vector2.zero)
        {
            dashVelocity = new Vector2(userInput.instance.moveInput.x * playerStats.dashSpeed,
                userInput.instance.moveInput.y * playerStats.dashSpeed);
        }
        else
        {
            dashVelocity = new Vector2((isFacingRight ? 1f : -1f) * playerStats.dashSpeed, 0f);
        }

        rb.linearVelocity = dashVelocity;
        trailRenderer.emitting = true;

        // ปล่อยให้ dash ค่อยๆ หน่วงความเร็วลงระหว่างทาง (แรงต้านน้ำ) แทนการคงที่ตลอดแล้วตัดจบ
        float elapsed = 0f;
        while (elapsed < playerStats.dashDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float decay = 1f - Mathf.Exp(-playerStats.deceleration * Time.fixedDeltaTime * 0.5f);
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, dashVelocity * 0.2f, decay);
            yield return new WaitForFixedUpdate();
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        trailRenderer.emitting = false;
        isDashing = false;
        rb.gravityScale = originGravity;

        yield return new WaitForSeconds(playerStats.dashCooldown);
        canDash = true;
    }

    #endregion
}