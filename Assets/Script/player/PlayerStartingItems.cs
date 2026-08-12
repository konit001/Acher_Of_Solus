using UnityEngine;

public class PlayerStartingItems : MonoBehaviour
{
    [Header("Starting Equipment")]
    [SerializeField] private BaseItemData firstWeapon; // ปรับชื่อตัวแปรให้ขึ้นต้นด้วยพิมพ์เล็กตามมาตรฐาน C#
    [SerializeField] private BaseItemData secondaryWeapon;

    void Start()
    {
        // ตรวจสอบว่า UiManager ถูกสร้างขึ้นมาแล้วเพื่อป้องกัน Error
        if (UiManager.instance == null)
        {
            Debug.LogError("UiManager.instance is missing in the scene!");
            return;
        }

        // เช็คว่ามีการใส่ข้อมูลอาวุธชิ้นแรกไว้หรือไม่
        if (firstWeapon != null)
        {
            UiManager.instance.AddItem(firstWeapon);
            UiManager.instance.UseItem(firstWeapon); // สวมใส่อาวุธชิ้นแรก
        }
        else
        {
            Debug.LogWarning("First weapon is not assigned in PlayerStartingItems.");
        }

        // เช็คว่ามีการใส่ข้อมูลอาวุธชิ้นที่สองไว้หรือไม่
        if (secondaryWeapon != null)
        {
            UiManager.instance.AddItem(secondaryWeapon);
            UiManager.instance.UseItem(secondaryWeapon); // สวมใส่อาวุธชิ้นที่สอง
        }
    }
}