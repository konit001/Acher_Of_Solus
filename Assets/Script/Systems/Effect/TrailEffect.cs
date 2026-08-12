using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    private Rigidbody2D rb;
    public TrailRenderer trail;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.linearVelocity.magnitude >= 0.1f)
        {
            trail.emitting = true;
        }
        else
        {
            trail.emitting = false;
        }
    }
}
