using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FrictionPuck: MonoBehaviour
{
    public float friction = 0.98f; // Value between 0 and 1; closer to 1 = slower decay
    public float minSpeedThreshold = 0.01f; // Stop if too slow

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > minSpeedThreshold)
        {
            rb.linearVelocity *= friction;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
