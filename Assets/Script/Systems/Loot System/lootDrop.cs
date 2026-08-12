using UnityEngine;
using System.Collections.Generic;

public class lootDrop : MonoBehaviour
{
    // public List<lootItem> lootTable = new List<lootItem>();
    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public Vector2 dropRadius = new Vector2(50f, 100f);
    Rigidbody2D rb;
    public Vector2 size;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Random.insideUnitCircle * Random.Range(dropRadius.x, dropRadius.y), ForceMode2D.Impulse);
    }
    void Update()
    {
        if(rb.linearVelocity != Vector2.zero)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime);
        }

        if(isGrounded())
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
    bool isGrounded()
    {
        return Physics2D.OverlapBox(transform.position , size , 0f , groundLayer);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.coral;
        Gizmos.DrawWireCube(transform.position , size);
    }
}

