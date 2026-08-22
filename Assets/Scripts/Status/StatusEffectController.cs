using System.Collections.Generic;
using UnityEngine;

// สถานะ 1 ตัวที่กำลังติดอยู่บนเป้าหมาย — เก็บเฉพาะค่าที่เปลี่ยนไปเรื่อยๆ
// ส่วนค่าคงที่ (duration, tickInterval, icon) อยู่ใน effectSO ไม่ซ้ำกันที่นี่
public class ActiveStatusEffect
{
    public StatusEffectSO effectSO;   // ต้นแบบของสถานะนี้
    public float remainingTime;       // เวลาที่ "เหลือ" ก่อนหมดอายุ ไม่ใช่ระยะเวลาทั้งหมด
    public float tickTimer;           // นับสะสมจนครบ tickInterval แล้วรีเซ็ตเป็น 0
    public int stackCount = 1;        // จำนวนชั้นที่ซ้อนอยู่ สูงสุดตาม effectSO.maxStack
    public GameObject spawnedVfx;     // เอฟเฟกต์ภาพที่ spawn ไว้ ต้อง Destroy ตอนสถานะหมด
    public float lockTimer;           // ตัวจับเวลาส่วนตัวของ SO — ตอนนี้มีแต่ Paralyzed ที่ใช้
}

// ติดตั้งบนตัวละครทุกตัวที่ "ติดสถานะได้" (ผู้เล่นและศัตรู)
// หน้าที่มีแค่: เก็บลิสต์สถานะที่ติดอยู่ เดินเวลาให้ทุกเฟรม แล้วเรียก callback ของ SO ตามจังหวะ
// ลอจิกของแต่ละสถานะไม่ได้อยู่ที่นี่ — อยู่ใน StatusEffectSO แต่ละคลาส (Fire/Water/Nature/Lightning)
public class StatusEffectController : MonoBehaviour, IStatusEffectReceiver
{
    private readonly List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();

    // ให้ HUD อ่านได้อย่างเดียว — มีแต่คลาสนี้ที่ add/remove ได้
    public IReadOnlyList<ActiveStatusEffect> ActiveEffects => activeEffects;

    private CharacterHealth characterHealth;

    void Awake()
    {
        characterHealth = GetComponent<CharacterHealth>();
    }

    // ทางเข้าเดียวของระบบ — ทั้งโซนสถานะและปุ่มทดสอบใน Manager วิ่งผ่านเมธอดนี้หมด
    public void ApplyEffect(StatusEffectSO effectSO)
    {
        if (effectSO == null) return; // ช่องใน Inspector ถูกปล่อยว่างไว้

        // ติดสถานะนี้อยู่แล้ว? ซ้อนชั้นเพิ่มแทนที่จะเพิ่มตัวใหม่ซ้ำ
        ActiveStatusEffect existing = activeEffects.Find(e => e.effectSO == effectSO);
        if (existing != null)
        {
            AddStack(existing, effectSO);
            return;
        }

        ActiveStatusEffect newEffect = CreateActiveEffect(effectSO);
        SetHpBarTint(effectSO, true);

        effectSO.OnApply(gameObject, newEffect);
        activeEffects.Add(newEffect);
    }

    void Update()
    {
        // วนถอยหลังเพื่อให้ลบตัวที่หมดอายุกลางลูปได้อย่างปลอดภัย
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (AdvanceEffect(activeEffects[i]))
                RemoveEffect(i);
        }
    }

    // เดินเวลาให้สถานะตัวหนึ่ง 1 เฟรม คืน true เมื่อหมดอายุแล้ว
    private bool AdvanceEffect(ActiveStatusEffect effect)
    {
        effect.remainingTime -= Time.deltaTime;
        effect.tickTimer += Time.deltaTime;

        // งานที่ต้องทำ "ทุกเฟรม" เช่น นับถอยหลัง lockTimer ของ Paralyzed
        effect.effectSO.OnEffectUpdate(gameObject, effect, Time.deltaTime);

        // งานที่ทำ "เป็นจังหวะ" เช่น ดาเมจต่อเนื่องของ Burn/Poison
        if (effect.tickTimer >= effect.effectSO.tickInterval)
        {
            effect.tickTimer = 0f;
            effect.effectSO.OnTick(gameObject, effect);
        }

        return effect.remainingTime <= 0f;
    }

    // ซ้อนชั้นเพิ่ม 1 (ไม่เกิน maxStack) และต่ออายุใหม่ถ้า SO ตั้งค่าไว้ให้ต่อ
    private void AddStack(ActiveStatusEffect existing, StatusEffectSO effectSO)
    {
        existing.stackCount = Mathf.Min(existing.stackCount + 1, effectSO.maxStack);
        if (effectSO.refreshDurationOnStack)
            existing.remainingTime = effectSO.duration;
    }

    // สร้างสถานะตัวใหม่ พร้อม spawn VFX ถ้า SO กำหนดไว้
    private ActiveStatusEffect CreateActiveEffect(StatusEffectSO effectSO)
    {
        ActiveStatusEffect newEffect = new ActiveStatusEffect
        {
            effectSO = effectSO,
            remainingTime = effectSO.duration,
            stackCount = 1
        };

        if (effectSO.vfxPrefab != null)
            newEffect.spawnedVfx = Instantiate(effectSO.vfxPrefab, transform);

        return newEffect;
    }

    // ย้อมสีหลอดเลือดตามธาตุตอนติดสถานะ และคืนสีเดิมตอนสถานะหมด
    // สถานะที่ไม่ใช่ debuff ธาตุ (elementDebuff = None) จะไม่แตะหลอดเลือดเลย
    private void SetHpBarTint(StatusEffectSO effectSO, bool isActive)
    {
        if (effectSO.elementDebuff == elementDebuffType.None) return;
        characterHealth?.ChangeHpBar(effectSO.elementDebuff, isActive);
    }

    // เก็บกวาดให้ครบตอนสถานะหมด: ลบ VFX → คืนสีหลอด → บอก SO → เอาออกจากลิสต์
    private void RemoveEffect(int index)
    {
        ActiveStatusEffect effect = activeEffects[index];

        if (effect.spawnedVfx != null)
            Destroy(effect.spawnedVfx);

        SetHpBarTint(effect.effectSO, false);

        effect.effectSO.OnRemove(gameObject, effect);
        activeEffects.RemoveAt(index);
    }
}
