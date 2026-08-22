using UnityEngine;

public class GroundEnemyCore : Enemy
{
[Header("Checker")]
public GameObject wallCheck;
public GameObject groundCheck;
public LayerMask groundLayer;
public LayerMask wallLayer;

// แยก Radius ออกจากกัน
public float groundCheckRadius = 0.2f; 
public float wallCheckRadius = 0.2f;

public bool IsGrounded()
{
    return Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);
}

public bool IsWall()
{
    return Physics2D.OverlapCircle(wallCheck.transform.position, wallCheckRadius, wallLayer);
}

protected override void OnDrawGizmosSelected()
{
    Gizmos.color = Color.magenta;
    if (wallCheck != null)
        Gizmos.DrawWireSphere(wallCheck.transform.position, wallCheckRadius);
    if (groundCheck != null)
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
}
}
