// using UnityEngine;

// public class EnemyBullet : MonoBehaviour
// {
//     private float speed;
//     private float lifetime;
//     public LayerMask hitLayers; 

//     private enemyCombat enemyCombatRef; // เก็บข้อมูลอ้างอิงศัตรูผู้ยิง

//     // เพิ่ม enemyCombat เข้ามาใน Setup
//     public void Setup(enemyCombat owner, float bulletSpeed, float bulletLifetime)
//     {
//         enemyCombatRef = owner;
//         speed = bulletSpeed;
//         lifetime = bulletLifetime;
//         Destroy(gameObject, lifetime);
//     }

//     void Update()
//     {
//         float moveDistance = speed * Time.deltaTime;
//         RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, moveDistance, hitLayers);

//         if (hit.collider != null && !hit.collider.isTrigger)
//         {
//             PlayerHealth player = hit.collider.GetComponent<PlayerHealth>();
//             if (player != null && enemyCombatRef != null)
//             {
//                 // สั่งคำนวณดาเมจผ่านตัวศัตรูที่ยิงกระสุนลูกนี้ออกมา
//                 enemyCombatRef.DamageCalculation(player);
//             }

//             Destroy(gameObject);
//             return;
//         }

//         transform.Translate(Vector2.right * moveDistance);
//     }
// }