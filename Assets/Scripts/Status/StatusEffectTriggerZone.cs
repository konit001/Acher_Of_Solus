using System.Collections.Generic;
using UnityEngine;

// พื้นที่ที่บังคับติดสถานะ (Burn/Freeze/Poison/ฯลฯ) ให้กับทุกอย่างที่มี StatusEffectController เดินผ่าน
// ใช้ทำโซนอันตราย เช่น กองไฟ หนองพิษ หรือพื้นน้ำแข็ง
[RequireComponent(typeof(Collider2D))]
public class StatusEffectTriggerZone : MonoBehaviour
{
    public StatusEffectSO effectToApply; // สถานะที่จะยัดให้ (ปล่อยว่างได้ Controller กันไว้แล้ว)
    public LayerMask affectedLayers;     // โซนนี้มีผลกับ layer ไหนบ้าง

    [Header("Repeat while inside zone")]
    public bool reapplyWhileInside = false; // ปิด = ติดครั้งเดียวตอนเดินเข้า
    public float reapplyInterval = 1f;      // เปิด = ยัดซ้ำทุกกี่วินาทีระหว่างยืนแช่อยู่

    // จับเวลาแยกรายตัว ว่าใครยืนอยู่ในโซนมานานเท่าไรนับจากโดนยัดครั้งล่าสุด
    // จะมีข้อมูลเฉพาะตอน reapplyWhileInside เปิดอยู่เท่านั้น
    private readonly Dictionary<IStatusEffectReceiver, float> stayTimers = new Dictionary<IStatusEffectReceiver, float>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!TryGetReceiver(other, out IStatusEffectReceiver receiver)) return;

        receiver.ApplyEffect(effectToApply);

        if (reapplyWhileInside)
            stayTimers[receiver] = 0f; // เริ่มจับเวลารอบถัดไป
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!reapplyWhileInside) return;
        if (!TryGetReceiver(other, out IStatusEffectReceiver receiver)) return;

        // ไม่มีชื่อใน stayTimers = เข้ามาตั้งแต่ตอนฟีเจอร์ยังปิดอยู่ ข้ามไปจนกว่าจะเดินออกแล้วเข้าใหม่
        if (!stayTimers.TryGetValue(receiver, out float elapsed)) return;

        // อ่านค่าครั้งเดียว บวกในตัวแปร local แล้วเขียนกลับครั้งเดียว
        elapsed += Time.deltaTime;
        if (elapsed >= reapplyInterval)
        {
            elapsed = 0f;
            receiver.ApplyEffect(effectToApply);
        }

        stayTimers[receiver] = elapsed;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // ตั้งใจไม่เช็ก layer ตรงนี้ เพื่อให้เก็บกวาดได้เสมอ แม้เป้าหมายจะเปลี่ยน layer ระหว่างอยู่ในโซน
        IStatusEffectReceiver receiver = other.GetComponent<IStatusEffectReceiver>();
        if (receiver != null)
            stayTimers.Remove(receiver);
    }

    // guard ชุดเดียวที่ Enter กับ Stay ใช้ร่วมกัน: ต้องอยู่ layer ที่สนใจ และต้องรับสถานะได้จริง
    private bool TryGetReceiver(Collider2D other, out IStatusEffectReceiver receiver)
    {
        receiver = null;
        if ((affectedLayers.value & (1 << other.gameObject.layer)) == 0) return false;

        receiver = other.GetComponent<IStatusEffectReceiver>();
        return receiver != null;
    }
}
