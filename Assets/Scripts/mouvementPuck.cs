using UnityEngine;
using System.Collections;
using Unity.Netcode; // namespace pour utiliser Netcode

using Unity.Netcode.Components;

public class mouvementPuck : NetworkBehaviour
{
    private Rigidbody2D rb;
    public static mouvementPuck instance;
    [SerializeField] private float maxSpeed = 15f; // Limite la vitesse
    [SerializeField] private float bounceFactor = 0.95f; // Perte d’énergie aux rebonds
    [SerializeField] private float forceMultiplier = 100f; // Multiplicateur pour ajuster la force
    [SerializeField] private float baseForce = 200f; // Force de base pour éviter un puck immobile
    float distance = 8.5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }






    void Update()
    {
        if (!IsServer) return;
        if (!GameManager.instance.partieEnCours) return;


        // Limite la vitesse max
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (transform.position.x < -distance) 
        {
            ScoreManager.instance.AugmenteScoreClient();
            LancerPuckMilieu();

        }


        if(transform.position.x > distance)
        {
            ScoreManager.instance.AugmenteHoteScore();
            LancerPuckMilieu();
        }
    }


    public void LancerPuckMilieu()
    {
        transform.position = new Vector3(0, 0, 0);
        GetComponent<Rigidbody2D>().linearVelocity = new Vector3(0, 0, 0);
        if (GameManager.instance.partieTerminee) return;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si le puck touche un joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            // Récupérer le script mouvementJoueur du joueur
            mouvementJoueur playerMovement = collision.gameObject.GetComponent<mouvementJoueur>();
            if (playerMovement != null)
            {
                // Calculer la direction du contact entre joueur et puck
                Vector2 direction = (rb.position - (Vector2)collision.transform.position).normalized;

                // Obtenir la vitesse du joueur
                float playerSpeed = playerMovement.Velocity.magnitude;

                // Calculer la force à appliquer (force de base + proportionnelle à la vitesse du joueur)
                float appliedForce = baseForce + (playerSpeed * forceMultiplier);

                // Appliquer la force au puck
                rb.AddForce(direction * appliedForce);
                Debug.Log($"Collision avec joueur, force appliquée: {appliedForce}");
            }
        }
        // Si le puck touche un mur
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Réduire la vitesse du puck en appliquant le bounceFactor
            rb.linearVelocity *= bounceFactor;
            Debug.Log($"Collision avec mur, vitesse après rebond: {rb.linearVelocity.magnitude}");
        }
        else
        {
            // Débogage pour identifier les collisions non gérées
            Debug.Log($"Collision avec {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        }
    }
}