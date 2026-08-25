using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class StatusEffectEntry
{
    public StatusEffectSO effectSO;
    public StatusEffectType effectType;
    public int Priority;
}

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance;

    public List<StatusEffectEntry> Effect = new List<StatusEffectEntry>();

    public StatusEffectController Player;
    public List<StatusEffectController> Enemy = new List<StatusEffectController>();

    [Header("Debug")]
    public bool debugApplyOnHit = true;   // ปิด log ได้จาก Inspector เมื่อไล่เสร็จแล้ว

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player = playerObject.GetComponent<StatusEffectController>();
        }

        GameObject[] EnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in EnemyObjects)
        {
            StatusEffectController controller = enemyObject.GetComponent<StatusEffectController>();
            if (controller != null) // skip enemies without the component
                Enemy.Add(controller);
        }
    }

    // ───── ติดสถานะจากการโจมตี ─────

    // ทางเข้าเดียวของระบบ เรียกจาก DamageCalculation ทั้งฝั่งผู้เล่นและศัตรู หลังดาเมจลงแล้ว
    // luck = โอกาสติดสถานะเป็นเปอร์เซ็นต์ (0-100) สเกลเดียวกับ CritRate
    public void TryApplyOnHit(GameObject target, ElementType element, float luck)
    {
        if (target == null)
        {
            Log("[1] target = null → จบ");
            return;
        }
        Log($"[1] เป้าหมาย = {target.name} | ธาตุ = {element} | luck = {luck}");

        if (!RollLuck(luck)) return;              // [2] log อยู่ข้างใน

        StatusEffectSO effectSO = GetEffectByElement(element);
        Log($"[3] หาสถานะของธาตุ {element} → {(effectSO != null ? effectSO.name : "ไม่เจอ (null)")}");
        if (effectSO == null) return;             // ธาตุนี้ยังไม่ได้ผูกสถานะไว้ในลิสต์ Effect

        IStatusEffectReceiver receiver = target.GetComponent<IStatusEffectReceiver>();
        Log($"[4] StatusEffectController บน {target.name} → {(receiver != null ? "เจอ" : "ไม่เจอ (null)")}");
        if (receiver == null) return;

        receiver.ApplyEffect(effectSO);
        Log($"[5] ใส่ '{effectSO.name}' ให้ {target.name} สำเร็จ");
    }

    // สุ่มแบบเดียวกับคริตใน DamageCalculation ทั้งสองฝั่ง — โชค 0 = ไม่ติดเลย, 100 = ติดทุกครั้ง
    private bool RollLuck(float luck)
    {
        if (luck <= 0f)
        {
            Log("[2] luck = 0 → ไม่สุ่ม ไม่มีทางติด");
            return false;
        }

        float roll = Random.Range(0f, 100f);
        bool pass = roll <= luck;
        Log($"[2] สุ่มได้ {roll:F1} เทียบ luck {luck} → {(pass ? "ผ่าน" : "ไม่ผ่าน")}");
        return pass;
    }

    // log ชั่วคราวสำหรับไล่หาว่าติดสถานะตกที่ด่านไหน — ปิดได้ที่ debugApplyOnHit
    private void Log(string message)
    {
        if (debugApplyOnHit) Debug.Log($"<color=#00d0ff><b>[OnHit]</b></color> {message}");
    }

    // ธาตุที่โจมตี → สถานะที่ควรติด
    // ถ้าธาตุเดียวกันมีหลายตัวในลิสต์ ตัวที่ Priority สูงสุดชนะ (ตรรกะเดียวกับ CharacterHealth.ApplyHighestPriorityColor)
    public StatusEffectSO GetEffectByElement(ElementType element)
    {
        if (element == ElementType.None) return null;

        StatusEffectEntry best = null;

        foreach (StatusEffectEntry entry in Effect)
        {
            if (entry.effectSO == null) continue;
            if (entry.effectSO.element != element) continue;

            if (best == null || entry.Priority > best.Priority)
                best = entry;
        }

        return best?.effectSO;
    }

#if UNITY_EDITOR
    // ───── ปุ่มทดสอบ (Editor เท่านั้น ไม่ติดไปกับ build) ─────

    void Update()
    {
        if (Keyboard.current == null) return;   // กันเครื่องที่ไม่มีคีย์บอร์ด

        if (Keyboard.current.digit1Key.wasPressedThisFrame) ApplyTest(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ApplyTest(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ApplyTest(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) ApplyTest(3);
    }

    // ยิงสถานะ Effect[index] ใส่ผู้เล่นและศัตรูตัวแรกแบบ 100% — ข้ามการสุ่มโชค ใช้ดูผลเร็วๆ
    private void ApplyTest(int index)
    {
        if (index < 0 || index >= Effect.Count) return;

        StatusEffectSO effectSO = Effect[index].effectSO;
        if (effectSO == null) return;

        Player?.ApplyEffect(effectSO);
        if (Enemy.Count > 0) Enemy[0]?.ApplyEffect(effectSO);
    }
#endif
}
