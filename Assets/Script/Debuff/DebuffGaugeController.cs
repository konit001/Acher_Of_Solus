using UnityEngine;
using System.Collections.Generic;

public class DebuffGaugeController : MonoBehaviour
{
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float gaugePerHit = 10f;
    [SerializeField] private DebuffDatabase debuffDatabase;

    private DebuffController debuffController;
    private readonly Dictionary<ElementType, float> gauges = new Dictionary<ElementType, float>();

    private void Awake()
    {
        debuffController = GetComponent<DebuffController>();
    }

    public void AddGauge(ElementType element)
    {
        if (element == ElementType.None || element == ElementType.ElementelLess) return;
        if (debuffDatabase == null || debuffController == null)
        {
            Debug.LogWarning($"[DebuffGauge] {gameObject.name}: ขาด reference debuffDatabase หรือ debuffController อยู่บน GameObject นี้");
            return;
        }

        gauges.TryGetValue(element, out float current);
        current += gaugePerHit;
        Debug.Log($"[DebuffGauge] {gameObject.name}: เกจธาตุ {element} = {current}/{maxGauge}");

        if (current >= maxGauge)
        {
            DebuffEffect effect = debuffDatabase.GetRandomDebuff(element);
            if (effect != null)
            {
                Debug.Log($"[DebuffGauge] {gameObject.name}: เกจธาตุ {element} เต็ม! สุ่มได้ดีบัฟ '{effect.debuffName}' -> apply");
                debuffController.ApplyDebuff(effect);
            }
            else
            {
                Debug.LogWarning($"[DebuffGauge] {gameObject.name}: เกจธาตุ {element} เต็มแต่ไม่พบดีบัฟใน DebuffDatabase สำหรับธาตุนี้");
            }
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
