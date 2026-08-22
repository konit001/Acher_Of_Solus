using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "ItemData/Weapon/Melee")]
public class MeleeWeaponData : weaponsData // สืบทอดจาก weaponsData เช่นกัน
{
    [Header("Melee Specific")]
    public float MeleeAttack;
    public Vector2 hitBoxSize;      // ขนาดของกล่องโจมตี (OverlapBox)
    public float knockbackForce;    // แรงผลักศัตรูให้กระเด็น
}