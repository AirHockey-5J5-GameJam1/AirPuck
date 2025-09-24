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

    public GameObject dernierJoueurAToucher;

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
            var joueur = dernierJoueurAToucher?.GetComponent<mouvementJoueur>();
            ScoreManager.instance.AugmenteScoreClient(joueur);
            LancerPuckMilieu();

        }


        if (transform.position.x > distance)
        {   
            var joueur = dernierJoueurAToucher?.GetComponent<mouvementJoueur>();
            ScoreManager.instance.AugmenteHoteScore(joueur);
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
                // Retenir le dernier joueur qui a touché le puck
                dernierJoueurAToucher = collision.gameObject;

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


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            print("tachetouille");
            // 1. Joue l'animation "break"
            Animator anim = collision.gameObject.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Break");
            }

            // 2. Donne le boost à la dernière personne qui a touché le puck
            if (dernierJoueurAToucher != null)
            {
                mouvementJoueur joueur = dernierJoueurAToucher.GetComponent<mouvementJoueur>();
                if (joueur != null)
                {
                    AppliquerBoostAleatoire(joueur);
                }
            }
            
            if (collision.TryGetComponent<NetworkObject>(out var netObj))
            {
                if (IsServer) // Seulement le serveur peut le despawn
                {
                    // petit délai avant destruction
                    StartCoroutine(DelayedDespawn(netObj, 1f));
                }
            }
        }
    }

    IEnumerator DelayedDespawn(NetworkObject netObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true); // supprime sur le réseau et local
        }
    }

    void AppliquerBoostAleatoire(mouvementJoueur joueur)
    {
        int boostType = Random.Range(0, 3); // 0, 1 ou 2

        switch (boostType)
        {
            case 0:
                StartCoroutine(BoostTaille(joueur));
                break;
            case 1:
                StartCoroutine(BoostPuckVitesse());
                break;
            case 2:
                StartCoroutine(BoostDoublePoint(joueur));
                break;
        }
    }




    IEnumerator BoostTaille(mouvementJoueur joueur)
    {

        joueur.transform.localScale *= 1.5f;
        yield return new WaitForSeconds(5f);
        joueur.transform.localScale /= 1.5f;
    }


    IEnumerator BoostPuckVitesse()
    {
        print("boost vitesse");
        float originalSpeed = maxSpeed;
        maxSpeed *= 2f;
        yield return new WaitForSeconds(5f);
        maxSpeed = originalSpeed;
    }
    
    IEnumerator BoostDoublePoint(mouvementJoueur joueur)
    {
        print("double point");
        joueur.doublePointActif = true;
        yield return new WaitForSeconds(15f);
        joueur.doublePointActif = false;
    }

}