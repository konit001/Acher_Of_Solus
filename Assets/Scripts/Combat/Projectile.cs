using System;
using UnityEngine;

public class Projectile : Core, IPooledObject
{
    // บัฟเฟอร์ร่วมของทุกกระสุน — Physics query ทำงานบนเธรดหลักทีละนัด จึงใช้ตัวเดียวกันได้
    // ขนาด 4 พอ เพราะสนใจแค่ตัวที่ใกล้ที่สุด
    private static readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[4];

    private float speed;
    private float lifetime;
    private ContactFilter2D hitFilter;
    private Action<Collider2D> onHitCallback;

    private float lifeTimer;

    // BulletState อ่านค่านี้เพื่อยืดแอนิเมชันให้จบพอดีตอนกระสุนหมดอายุ
    public float Lifetime => lifetime;

    [Header("State")]
    public State bullet;

    // เลเยอร์สิ่งกีดขวางที่กระสุนทุกนัดต้องชนเสมอ ไม่ว่าใครยิง (พื้น/กำแพง) — ค่าเริ่มต้นคือ Ground (layer 6)
    [Header("Obstacles")]
    public LayerMask obstacleLayers = 1 << 6;

    // ฟังก์ชันนี้รับค่าคอนฟิกตอนยิง และรับคำสั่งว่าจะให้ทำอะไรตอนชน (Callback)
    public void Setup(float bulletSpeed, float bulletLifetime, LayerMask targetLayers, Action<Collider2D> onHitAction)
    {
        speed = bulletSpeed;
        lifetime = bulletLifetime;
        hitFilter = BuildHitFilter(targetLayers | obstacleLayers);
        onHitCallback = onHitAction;

        // เข้า State ตรงนี้ ไม่ใช่ใน OnObjectSpawn เพราะ BulletState ต้องรู้ lifetime ตอน Enter()
        // แต่ ObjectPooling เรียก OnObjectSpawn ก่อนที่คนยิงจะเรียก Setup เสมอ
        // forceReset = true จำเป็น เพราะกระสุนที่ใช้ซ้ำยังค้าง state เดิมอยู่ ถ้าไม่บังคับจะข้าม Enter()
        stateMacines.set(bullet, true);
    }

    // สร้าง State Machine ครั้งเดียวตอนถูก Instantiate เข้า pool
    // ไม่ทำใน OnObjectSpawn เพราะกระสุนถูก spawn ถี่มาก และ setupInstances()
    // มี GetComponentsInChildren ที่ alloc array ใหม่ทุกครั้งที่เรียก
    void Awake()
    {
        setupInstances();
    }

    public void OnObjectSpawn()
    {
        lifeTimer = 0f; // รีเซ็ตเวลาทุกครั้งที่ถูกดึงมาใช้ใหม่
    }

    void Update()
    {
        // กระสุนบางตัวอาจไม่ได้ผูก State ไว้ใน Inspector — ปล่อยให้บินต่อได้โดยไม่พังทั้งเกม
        stateMacines.state?.Do();
        // 1. นับเวลา Lifetime
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
        {
            Despawn();
            return;
        }

        // 2. เคลื่อนที่และเช็คการชน
        float moveDistance = speed * Time.deltaTime;

        if (TryGetHit(moveDistance, out Collider2D hitCollider))
        {
            // ส่ง Collider กลับไปให้คนยิง (Player หรือ Enemy) เป็นคนคำนวณดาเมจ
            onHitCallback?.Invoke(hitCollider);
            Despawn();
            return;
        }

        transform.Translate(Vector2.right * moveDistance);
    }

    // เงื่อนไขว่ากระสุนนัดนี้ "นับว่าโดนอะไร" — แก้ที่เดียวจบ
    // useTriggers = false สำคัญมาก: ตัวละครมีวงตรวจจับ/โซน interact เป็น trigger ครอบ hitbox จริงอยู่
    // ถ้าไม่ปิด กระสุนจะไปหยุดที่วงครอบแทนที่จะถึงตัวจริง
    private static ContactFilter2D BuildHitFilter(LayerMask targetLayers)
    {
        return new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = targetLayers
        };
    }

    // ยิง ray ไปข้างหน้าเท่าระยะที่จะเคลื่อนที่เฟรมนี้ คืนตัวที่โดนก่อน
    // Physics2D เรียงผลลัพธ์ตามระยะให้อยู่แล้ว ตัวแรกจึงเป็นตัวที่ใกล้ที่สุด
    // อยากทำกระสุนทะลุหลายตัว ก็วนลูป hitBuffer ตรงนี้แทนการหยิบแค่ตัวแรก
    private bool TryGetHit(float distance, out Collider2D hitCollider)
    {
        int hitCount = Physics2D.Raycast(transform.position, transform.right, hitFilter, hitBuffer, distance);

        hitCollider = hitCount > 0 ? hitBuffer[0].collider : null;
        return hitCollider != null;
    }

    // ซ่อนแทนการ Destroy เพื่อให้ ObjectPooling เอากลับไปใช้ใหม่ได้
    private void Despawn()
    {
        gameObject.SetActive(false);
    }
}
