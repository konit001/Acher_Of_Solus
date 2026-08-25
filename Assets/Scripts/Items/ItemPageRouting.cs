// กติกาว่าไอเทมชิ้นไหนควรไปอยู่หน้าไหนใน UI — เขียนไว้ที่เดียว ใครอยากรู้ให้มาอ่านที่นี่
public static class ItemPageRouting
{
    // หน้า Equipment เก็บ Weapon กับ Artifact — ที่เหลือทั้งหมดอยู่หน้า Inventory
    public static bool BelongsToEquipmentPage(BaseItemData item)
    {
        if (item == null) return false;

        return item.itemType == ItemType.Weapon
            || item.itemType == ItemType.Artifact;
    }
}
