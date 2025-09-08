using UnityEngine;
using UnityEngine.InputSystem;

public class mouvementJoueur : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 lastPosition;
    private Rigidbody2D rb; // Rigidbody2D du joueur

    public float minX, maxX, minY, maxY;

    // Propriété publique pour accéder à la vitesse du joueur
    public Vector2 Velocity { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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

            // Déplacer le joueur
            transform.position = newPosition;

            // Calculer la vitesse
            Velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            // Mettre à jour la vitesse du Rigidbody2D pour la cohérence physique
            rb.linearVelocity = Velocity;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            Velocity = Vector2.zero; // Réinitialiser la vitesse quand le drag s'arrête
            rb.linearVelocity = Vector2.zero; // Arrêter le mouvement du joueur
        }
    }
}