using UnityEngine;

public enum Slot { Slot1, Slot2 }

public class weaponSlot : SlotBase
{
    public Slot slot;

    // ไม่ Clear() ตอน Awake — มี script แจกอาวุธเริ่มต้นและบังคับสวมใส่ให้ตั้งแต่เริ่มเกม

    void OnEnable()
    {
        if (WeaponProgressManager.instance != null)
            WeaponProgressManager.instance.OnWeaponUpgraded += OnWeaponUpgraded;

        if (!IsEmpty) ShowInfo();   // เปิดแผงทีหลังก็ยังเห็นค่าล่าสุด
    }

    void OnDisable()
    {
        if (WeaponProgressManager.instance != null)
            WeaponProgressManager.instance.OnWeaponUpgraded -= OnWeaponUpgraded;
    }

    // อาวุธเล่มเดียวกันโชว์อยู่ทั้งช่อง HUD และช่องในแผง Equipment จึงต้องรีเฟรชทั้งคู่
    private void OnWeaponUpgraded(weaponsData weapon)
    {
        if (currentItem == weapon) ShowInfo();
    }

    protected override void ShowInfo()
    {
        base.ShowInfo();            // ไอคอน + ชื่อ แล้วซ่อนกลุ่มเลเวลไว้ก่อน

        weaponsData weapon = currentItem as weaponsData;
        if (weapon == null || weapon.levelData == null) return;   // อาวุธที่ยังไม่ผูกตารางอัปเกรด โชว์แค่ชื่อพอ
        if (WeaponProgressManager.instance == null) return;

        ShowLevel(weapon);
        ShowMaterials(weapon);
        ShowUpgradeButton(weapon);
    }

    private void ShowLevel(weaponsData weapon)
    {
        int level = WeaponProgressManager.instance.GetLevel(weapon);
        int maxLevel = weapon.levelData.maxLevel;

        if (_Level != null) _Level.text = level + "/" + maxLevel;
        if (_LevelFill != null) _LevelFill.fillAmount = maxLevel > 0 ? (float)level / maxLevel : 0f;
    }

    private void ShowMaterials(weaponsData weapon)
    {
        if (_Materials == null) return;

        WeaponLevelData.UpgradeStep step = WeaponProgressManager.instance.GetNextStep(weapon);

        if (step == null || WeaponProgressManager.instance.IsMaxLevel(weapon))
        {
            _Materials.HideAll();          // เลเวลเต็มแล้วไม่ต้องโชว์วัสดุ
            return;
        }

        _Materials.ShowCosts(step.materials);
    }

    private void ShowUpgradeButton(weaponsData weapon)
    {
        if (_Upgrade == null) return;

        _Upgrade.onClick.RemoveAllListeners();
        _Upgrade.onClick.AddListener(() => WeaponProgressManager.instance.Upgrade(weapon));
        _Upgrade.interactable = WeaponProgressManager.instance.CanUpgrade(weapon);
    }

    protected override void OnRightClickInMenu()
    {
        UiManager.instance.UnEquipWeapon(slot);
    }
}
