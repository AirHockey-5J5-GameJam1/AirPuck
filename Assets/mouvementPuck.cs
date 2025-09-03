using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class mouvementPuck : MonoBehaviour
{
    public float randomForceInterval = 2f; // Time between random pushes
    public float randomForceStrength = 0.5f; // How strong the pushes are
    public float drag = 0.2f; // Slows puck over time (like air friction)

    private Rigidbody2D rb;
    private float timeSinceLastPush;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = drag; // Apply drag for air resistance
    }

    void Update()
    {
        timeSinceLastPush += Time.deltaTime;

        if (timeSinceLastPush >= randomForceInterval)
        {
            ApplyRandomPush();
            timeSinceLastPush = 0f;
        }
    }

    void ApplyRandomPush()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDirection * randomForceStrength, ForceMode2D.Impulse);
    }
}
