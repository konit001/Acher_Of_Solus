using UnityEngine;
using System.Collections.Generic;

// ตารางอัปเกรดของอาวุธ — แยกเป็น asset ต่างหากเพื่อให้อาวุธหลายเล่มใช้ตารางเดียวกันได้
// และปรับบาลานซ์ได้จากที่เดียว
[CreateAssetMenu(fileName = "WeaponLevelData", menuName = "ItemData/Weapon/LevelData")]
public class WeaponLevelData : ScriptableObject
{
    [System.Serializable]
    public class MaterialCost
    {
        public materialData material;
        public int amount = 1;
    }

    // ค่าใช้จ่ายในการอัปจากเลเวลหนึ่งไปเลเวลถัดไป
    [System.Serializable]
    public class UpgradeStep
    {
        public List<MaterialCost> materials = new List<MaterialCost>();
        public int coinCost;
        [Tooltip("ค่าสถานะของอาวุธเพิ่มกี่ % เมื่ออัปมาถึงเลเวลนี้")]
        public float statBonusPercent = 10f;
    }

    public int maxLevel = 10;

    [Tooltip("steps[0] = ค่าอัปจากเลเวล 0 ไปเลเวล 1")]
    public List<UpgradeStep> steps = new List<UpgradeStep>();

    // ค่าอัปจากเลเวลปัจจุบันไปเลเวลถัดไป
    // ถ้าตารางสั้นกว่า maxLevel ให้ใช้ค่าของขั้นสุดท้ายซ้ำไปเรื่อย ๆ
    public UpgradeStep GetStep(int currentLevel)
    {
        if (steps.Count == 0) return null;

        int index = Mathf.Clamp(currentLevel, 0, steps.Count - 1);
        return steps[index];
    }

    // โบนัสรวมของอาวุธที่เลเวลนี้ (%) = ผลรวม statBonusPercent ของทุกขั้นที่อัปผ่านมาแล้ว
    public float GetTotalBonusPercent(int level)
    {
        float total = 0f;

        for (int i = 0; i < level; i++)
        {
            UpgradeStep step = GetStep(i);
            if (step != null) total += step.statBonusPercent;
        }

        return total;
    }
}
