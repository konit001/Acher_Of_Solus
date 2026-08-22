using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    [Header("References")]
    public PlayerAttack playerAttack; // อ้างอิงไปที่สคริปต์บนตัวแม่

    void Awake()
    {
        // ถ้าลืมลากใส่ มันจะหาคอมโพเนนต์จากตัวแม่ (Parent) ให้เองอัตโนมัติ
        if (playerAttack == null)
        {
            playerAttack = GetComponentInParent<PlayerAttack>();
        }
    }

    // ฟังก์ชันนี้แหละที่ Animation Event จะมองเห็นและเรียกใช้
    public void CallSpearDamage()
    {
        if (playerAttack != null)
        {
            // สั่งให้ตัวแม่ทำดาเมจ
            playerAttack.TriggerSpearDamage();
        }
    }
}