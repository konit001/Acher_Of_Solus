using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "ItemData/Weapon/Ranged")]
public class RangedWeaponData : weaponsData // สืบทอดจาก weaponsData ของคุณ
{
    [Header("Ranged Specific")]
    public float rangedAttack;
    public GameObject bulletPrefab; // กระสุนที่ใช้
    public float bulletlifetime;
    public float bulletSpeed;
    public float fireRate;          // ความเร็วในการยิงรัว
    public int Enegy;             // จำนวนกระสุนในแม็กกาซีน
}