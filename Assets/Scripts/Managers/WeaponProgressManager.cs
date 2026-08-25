using UnityEngine;
using System.Collections.Generic;

// ความคืบหน้าของอาวุธ 1 เล่มขณะเล่น
// เก็บแยกจากไฟล์ .asset จึงไม่เขียนทับข้อมูลต้นฉบับของอาวุธ
public class WeaponProgress
{
    public int level;
    public List<int> unlockedSkillIds = new List<int>();   // เตรียมไว้ให้ระบบสกิลรอบถัดไป
}

public class WeaponProgressManager : MonoBehaviour
{
    public static WeaponProgressManager instance;

    [SerializeField] private CoinManager coinManager;

    // อาวุธชนิดเดียวกันแชร์ความคืบหน้ากัน (กระเป๋าก็ stack ไอเทมด้วย reference ของ asset อยู่แล้ว)
    private Dictionary<weaponsData, WeaponProgress> progressTable = new Dictionary<weaponsData, WeaponProgress>();

    // ยิงเมื่ออาวุธเล่มไหนอัปเกรดสำเร็จ — ช่อง HUD กับช่องในแผง Equipment จะได้รีเฟรชพร้อมกัน
    public event System.Action<weaponsData> OnWeaponUpgraded;

    private InventoryManager inventoryManager
        => InventoryManager.instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // ─── อ่านค่า ──────────────────────────────────────────────

    public WeaponProgress GetProgress(weaponsData weapon)
    {
        if (weapon == null) return null;

        WeaponProgress progress;
        if (!progressTable.TryGetValue(weapon, out progress))
        {
            progress = new WeaponProgress { level = weapon.level };   // เริ่มจากเลเวลที่ตั้งไว้ใน asset
            progressTable.Add(weapon, progress);
        }

        return progress;
    }

    public int GetLevel(weaponsData weapon)
    {
        WeaponProgress progress = GetProgress(weapon);
        return progress != null ? progress.level : 0;
    }

    // ค่าอัปจากเลเวลปัจจุบันไปเลเวลถัดไป — null ถ้าอาวุธยังไม่ได้ผูกตารางอัปเกรด
    public WeaponLevelData.UpgradeStep GetNextStep(weaponsData weapon)
    {
        if (weapon == null || weapon.levelData == null) return null;
        return weapon.levelData.GetStep(GetLevel(weapon));
    }

    public bool IsMaxLevel(weaponsData weapon)
    {
        if (weapon == null || weapon.levelData == null) return true;
        return GetLevel(weapon) >= weapon.levelData.maxLevel;
    }

    // มีวัสดุชิ้นนี้ในกระเป๋ากี่ชิ้น
    public int GetOwnedAmount(materialData material)
    {
        if (material == null || inventoryManager == null) return 0;
        return inventoryManager.GetAmount(material);
    }

    public bool HasEnoughMaterials(weaponsData weapon)
    {
        WeaponLevelData.UpgradeStep step = GetNextStep(weapon);
        if (step == null) return false;

        foreach (WeaponLevelData.MaterialCost cost in step.materials)
        {
            if (cost.material == null) continue;
            if (GetOwnedAmount(cost.material) < cost.amount) return false;
        }

        if (step.coinCost > 0 && coinManager != null && !coinManager.HasCoin(step.coinCost))
            return false;

        return true;
    }

    public bool CanUpgrade(weaponsData weapon)
    {
        return !IsMaxLevel(weapon) && HasEnoughMaterials(weapon);
    }

    // ตัวคูณค่าสถานะของอาวุธตามเลเวล เช่น เลเวล 2 ที่โบนัสขั้นละ 10% จะได้ 1.2
    public float GetStatMultiplier(weaponsData weapon)
    {
        if (weapon == null || weapon.levelData == null) return 1f;
        return 1f + weapon.levelData.GetTotalBonusPercent(GetLevel(weapon)) / 100f;
    }

    // ─── อัปเกรด ──────────────────────────────────────────────

    public void Upgrade(weaponsData weapon)
    {
        if (!CanUpgrade(weapon)) return;

        WeaponLevelData.UpgradeStep step = GetNextStep(weapon);

        foreach (WeaponLevelData.MaterialCost cost in step.materials)
        {
            if (cost.material == null || inventoryManager == null) continue;
            inventoryManager.RemoveItem(cost.material, cost.amount);
        }

        if (step.coinCost > 0 && coinManager != null)
            coinManager.SpendCoin(step.coinCost);

        GetProgress(weapon).level++;

        Debug.Log($"อัปเกรด {weapon.itemName} เป็นเลเวล {GetLevel(weapon)}/{weapon.levelData.maxLevel}");

        if (OnWeaponUpgraded != null) OnWeaponUpgraded(weapon);
    }
}
