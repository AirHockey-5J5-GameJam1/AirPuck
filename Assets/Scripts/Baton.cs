using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem; // Optionnel selon ton input system

public class Baton : NetworkBehaviour
{
    public float vitesse = 0.05f;
    public float minY;
    public float maxY;
    public float minX;
    public float maxX;

    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 lastPosition;
    private Rigidbody2D rb;

    public Vector2 Velocity { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        // Positionnement initial : Hôte à gauche, client à droite
        if (IsServer)
        {
            transform.position = new Vector3(-6f, 0f, 0f); // À ajuster selon ton terrain
        }
        else
        {
            transform.position = new Vector3(6f, 0f, 0f);
        }
    }

    void Update()
    {
        if (!IsLocalPlayer) return;

        if (Mouse.current == null || mainCamera == null) return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D hit = Physics2D.OverlapPoint(mousePosition);
            if (hit && hit.gameObject == gameObject)
            {
                offset = transform.position - mousePosition;
                isDragging = true;
                lastPosition = transform.position;
            }
        }

        if (isDragging)
        {
            Vector3 newPosition = mousePosition + offset;

            // Limites de mouvement du bâton
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
