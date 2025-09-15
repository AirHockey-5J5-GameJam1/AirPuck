using UnityEngine;
using Unity.Netcode; // namespace pour utiliser Netcode
using UnityEngine.SceneManagement; // namespace pour la gestion des scènes

public class GameManager : NetworkBehaviour //pour un network object
{
    public static GameManager instance;// Singleton pour parler au GameManager de n'importe où

    public bool partieEnCours { private set; get; } //permet de savoir si une partie est en cours
    public bool partieTerminee { private set; get; } // permet de savoir si une partie est terminée
    public GameObject Joueur1;
    public GameObject Joueur2;

    // Création du singleton si nécessaire
    void Awake()
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


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.Singleton.OnClientConnectedCallback += OnNouveauClientConnecte;
    }


    private void OnNouveauClientConnecte(ulong obj)
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            GameObject nouveauJoueur = Instantiate(Joueur1);
            nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
            nouveauJoueur.transform.position = new Vector3(-7f, 0.5f, 0f);
        }
        else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            GameObject nouveauJoueur = Instantiate(Joueur2);
            nouveauJoueur.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
            nouveauJoueur.transform.position = new Vector3(7f, 0.5f, 0f);
        }
    }


    // Fonction appelée pour le bouton qui permet de se connecter comme hôte
    public void LanceCommeHote()
    {
        NetworkManager.Singleton.StartHost();
    }

    // Fonction appelée pour le bouton qui permet de se connecter comme client
    public void LanceCommeClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    // L'hôte de la partie attend que 2 joueurs soient connectés pour lancer la partie
    // Seulement l'hôte exécute ce code
    // Aucune vérification si partie déjà en cours
    void Update()
    {
        if (!IsHost) return;
        if (partieEnCours) return;

            NouvellePartie();
    }

    // Activation d'une nouvelle partie lorsque 2 joueurs. On appelle la fonction de la balle qui
    // la place au milieu et qui lui donne une vélocité.
    public void NouvellePartie()
    {
        partieEnCours = true;
    }

   // Fonction appelée par le ScoreManager pour terminer la partie
    public void FinPartie()
    {
        partieTerminee = true;
    }

  // Fonction appelée par le bouton Recommencer pour recommencer une partie
    public void Recommencer()
    {
        NetworkManager.Singleton.Shutdown(); // On arrête le NetworkManager pour réinitialiser la partie
        Destroy(NetworkManager.gameObject);
        partieEnCours = false; // On remet la partie en cours à false
        SceneManager.LoadScene(0);// On recharge la scène de jeu
    }
}
