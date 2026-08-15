using UnityEngine;
using System.Collections.Generic;

// มิเรอร์ DebuffGaugeController — โครงพร้อมใช้ ยังไม่มีจุดเรียก AddGauge จริง
// (รอระบบที่เป็นแหล่งสร้างบัฟ เช่น potion/skill/artifact)
public class BuffGaugeController : MonoBehaviour
{
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float gaugePerHit = 10f;
    [SerializeField] private BuffDatabase buffDatabase;

    private BuffController buffController;
    private readonly Dictionary<ElementType, float> gauges = new Dictionary<ElementType, float>();

    private void Awake()
    {
        buffController = GetComponent<BuffController>();
    }

    public void AddGauge(ElementType element)
    {
        if (element == ElementType.None || element == ElementType.ElementelLess) return;
        if (buffDatabase == null || buffController == null) return;

        gauges.TryGetValue(element, out float current);
        current += gaugePerHit;

        if (current >= maxGauge)
        {
            BuffEffect effect = buffDatabase.GetRandomBuff(element);
            if (effect != null) buffController.ApplyBuff(effect);
            current = 0f;
        }

        gauges[element] = current;
    }

    // ใช้โดย UI เพื่ออ่านสัดส่วนเกจปัจจุบัน (0-1) ของธาตุนั้นๆ
    public float GetGaugeRatio(ElementType element)
    {
        gauges.TryGetValue(element, out float current);
        return maxGauge <= 0f ? 0f : Mathf.Clamp01(current / maxGauge);
    }
}
