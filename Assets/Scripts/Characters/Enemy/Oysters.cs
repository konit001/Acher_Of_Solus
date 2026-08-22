// using UnityEngine;

// public class Oysters : FloatingEnemyCore
// {
//     [Header("State Machine & Combat")]
//     public State patrolState;
//     public State shootState;
//     protected override void Awake()
//     {
//         // เรียกใช้ Awake ของคลาสแม่ (Enemy) เพื่อเก็บค่า startPoint
//         base.Awake();

//         // ใส่โค้ดตั้งค่าเริ่มต้นเฉพาะของ Oysters เพิ่มเติมที่นี่ (ถ้ามี)
//     }

//     protected override void Start()
//     {
//         // เรียกใช้ Start ของคลาสแม่ เพื่อรัน setupInstances() และตั้งค่าทิศทางเบื้องต้น
//         base.Start();
//         stateMacines.set(patrolState);
//         // ตัวอย่างการกำหนด State หากมีการสร้าง State แยกเฉพาะสำหรับตัวนี้
//         // patrolState = new OystersPatrolState(this, stateMacines, "Patrol");
//         // shootState = new OystersShootState(this, stateMacines, "Shoot");
//     }

//     protected override void Update()
//     {
//         // เรียกใช้ Update ของคลาสแม่ เพื่อให้มันรัน selectState() และ stateMacines.state.Do() อัตโนมัติ
//         base.Update();
//         stateMacines.state.Do();


//         // ใส่การทำงานเพิ่มเติมเฉพาะของ Oysters ที่นี่ (ถ้ามี)
//     }
//     protected override void selectState()
//     {
//         if (combat != null && combat.DetectPlayer() && shootState != null)
//         {
//             stateMacines.set(shootState);
//         }
//         else
//         {
//             stateMacines.set(patrolState);
//         }
//     }


//     // // บังคับต้อง Override เพราะคลาสแม่ประกาศเป็น abstract ไว้
//     // protected override void OnDrawGizmosSelected()
//     // {
//     //     // ใช้สำหรับวาดเส้นไกด์ไลน์ในหน้า Scene View เช่น ระยะการมองเห็น หรือ ระยะโจมตี
//     //     Gizmos.color = Color.red;

//     //     // ตัวอย่าง: วาดวงกลมแสดงระยะโจมตี 3 หน่วย
//     //     // Gizmos.DrawWireSphere(transform.position, 3f); 
//     // }
// }