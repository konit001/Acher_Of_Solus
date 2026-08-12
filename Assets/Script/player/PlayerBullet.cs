// using UnityEngine;

// public class PlayerBullet : MonoBehaviour
// {
//     [Header("Bullet Settings")]
//     private float lifetime;     
//     private float speed;            
//     private PlayerAttack playerAttackRef;
//     private float damageMultiple;
//     private ElementType elementType;
    
//     [Tooltip("ใส่ Layer Enemy, Ground, และ Wall ไว้ในนี้")]
//     public LayerMask hitLayers;

//     public void Setup(PlayerAttack playerAttack, float multiple, float bulletSpeed, float bulletlifetime , ElementType element)
//     {
//         playerAttackRef = playerAttack;
//         damageMultiple = multiple;
//         speed = bulletSpeed; 
//         lifetime = bulletlifetime;
//         elementType = element;

//         // สั่งทำลายตัวเองล่วงหน้าตามเวลา lifetime
//         Destroy(gameObject, lifetime);
//     }

//     void Update()
//     {
//         float moveDistance = speed * Time.deltaTime;
        
//         // ใช้ Raycast เช็คการชนทั้งหมดในเส้นทาง
//         RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, moveDistance, hitLayers);
        
//         if(hit.collider != null)
//         {
//             if (!hit.collider.isTrigger)
//             {
//                 HitTarget(hit.collider);
//                 return; // หยุดการขยับ
//             }
//         }

//         transform.Translate(Vector2.right * moveDistance);
//     }

//     private void HitTarget(Collider2D hitCollider)
//     {
//         EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
        
//         if (enemy != null && playerAttackRef != null)
//         {
//             playerAttackRef.DamageCalculation(enemy, damageMultiple, elementType);
//         }

//         Destroy(gameObject);
//     }
// }