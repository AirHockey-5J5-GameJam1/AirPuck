using UnityEngine;
using UnityEngine.InputSystem;

public class mouvementJoueur : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 lastPosition;
    private Vector3 velocity;

    public float minX, maxX, minY, maxY;
    public Rigidbody2D puck; // Reference to the puck
    public float hitDistance; // Distance to check for collision
    public float forceMultiplier; // Tune this

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject && targetObject.gameObject == gameObject)
            {
                offset = transform.position - mousePosition;
                isDragging = true;
                lastPosition = transform.position;
            }
        }

        if (isDragging)
        {
            Vector3 newPosition = mousePosition + offset;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            // Move the stick
            transform.position = newPosition;

            // Compute velocity
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;

            // Check if the puck is within hit range
            if (Vector2.Distance(transform.position, puck.position) <= hitDistance)
            {
                // Apply force to the puck
                puck.linearVelocity = velocity * forceMultiplier;

            }
        }
    }
}
