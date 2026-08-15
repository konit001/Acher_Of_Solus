using UnityEngine;

// marker subclass — ดีบัฟที่ไม่เกี่ยวกับธาตุ ไม่ผ่านเกจ/DebuffDatabase
// (รูปแบบเดียวกับ materialData/potionData ในโปรเจกต์ที่ subclass ว่างเปล่าเพื่อแยกชนิด)
[CreateAssetMenu(fileName = "GeneralDebuffEffect", menuName = "BuffEffect/Debuff/General")]
public class GeneralDebuffEffect : DebuffEffect
{
}
