using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class mouvementJoueur : NetworkBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 lastPosition;
    private Rigidbody2D rb;

    public float minX, maxX, minY, maxY;

    public Vector2 Velocity { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        GestionDeplacement();
        
    }

    public override void OnNetworkDespawn()

    {
        base.OnNetworkDespawn();
        if (IsServer) Destroy(gameObject);
        print("le joueur est despawn");
    }


    void GestionDeplacement()
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

            transform.position = newPosition;

            Velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            rb.linearVelocity = Velocity;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            Velocity = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
