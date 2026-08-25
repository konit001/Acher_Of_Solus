using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    // บัฟเฟอร์ร่วมของทุกกระสุน — Physics query ทำงานบนเธรดหลักทีละนัด จึงใช้ตัวเดียวกันได้
    // ขนาด 4 พอ เพราะสนใจแค่ตัวที่ใกล้ที่สุด
    private static readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[4];

    private float speed;
    private float lifetime;
    private ContactFilter2D hitFilter;
    private Action<Collider2D> onHitCallback;

    private float lifeTimer;

    // ฟังก์ชันนี้รับค่าคอนฟิกตอนยิง และรับคำสั่งว่าจะให้ทำอะไรตอนชน (Callback)
    public void Setup(float bulletSpeed, float bulletLifetime, LayerMask targetLayers, Action<Collider2D> onHitAction)
    {
        speed = bulletSpeed;
        lifetime = bulletLifetime;
        hitFilter = BuildHitFilter(targetLayers);
        onHitCallback = onHitAction;
    }

    public void OnObjectSpawn()
    {
        lifeTimer = 0f; // รีเซ็ตเวลาทุกครั้งที่ถูกดึงมาใช้ใหม่
    }

    void Update()
    {
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
