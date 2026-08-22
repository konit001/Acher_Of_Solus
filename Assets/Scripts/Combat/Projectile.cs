using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    private float speed;
    private float lifetime;
    private LayerMask hitLayers;
    private Action<Collider2D> onHitCallback; 
    
    private float lifeTimer;

    // ฟังก์ชันนี้รับค่าคอนฟิกตอนยิง และรับคำสั่งว่าจะให้ทำอะไรตอนชน (Callback)
    public void Setup(float bulletSpeed, float bulletLifetime, LayerMask targetLayers, Action<Collider2D> onHitAction)
    {
        speed = bulletSpeed;
        lifetime = bulletLifetime;
        hitLayers = targetLayers;
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
            gameObject.SetActive(false); // หมดเวลาให้ซ่อน (เก็บเข้า Pool) แทนการ Destroy
            return;
        }

        // 2. เคลื่อนที่และเช็คการชน
        float moveDistance = speed * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, moveDistance, hitLayers);

        if (hit.collider != null && !hit.collider.isTrigger)
        {
            // ส่ง Collider กลับไปให้คนยิง (Player หรือ Enemy) เป็นคนคำนวณดาเมจ
            onHitCallback?.Invoke(hit.collider); 
            
            gameObject.SetActive(false); // ชนแล้วซ่อน (เก็บเข้า Pool)
            return;
        }

        transform.Translate(Vector2.right * moveDistance);
    }
    
}