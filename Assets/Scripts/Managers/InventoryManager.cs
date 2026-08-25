using UnityEngine;

// หน้า Inventory — เก็บของทั่วไปทั้งหมด (material / consumable / coin / lootbag)
// ที่เก็บของกับการวาดช่องอยู่ใน ItemStorage คลาสนี้เหลือแค่การเป็น singleton
public class InventoryManager : ItemStorage
{
    public static InventoryManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
}
