using TMPro;
using Unity.Netcode;
using UnityEngine;
public class ScoreManager : NetworkBehaviour
{

    public static ScoreManager instance;
    [SerializeField] private TMP_Text scoreTxt; // R�f�rence � la zone qui affiche le texte
    [SerializeField] private int pointageCible; // Le pointage � atteindre pour gagner
    private NetworkVariable<int> scoreHote = new NetworkVariable<int>(); // Score de l'h�te (variable r�seau)
    private NetworkVariable<int> scoreClient = new NetworkVariable<int>(); // Score du client (variable r�seau)
    public GameObject pannelVictoire; // R�f�rence au panel pour la victoire
    public GameObject pannelDefaite; // R�f�rence au panel pour la d�faite

    // Start is called once before the first execution of Update after the MonoBehaviour is created

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



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            scoreHote.Value = 0;
            scoreClient.Value = 0;
        }

        scoreHote.OnValueChanged += OnChangementPointageHote;
        scoreClient.OnValueChanged += OnChangementPointageClient;
    }

    /* M�thode appel�e lors de la d�sactivation de l'objet r�seau
    - Se d�sabonne des �v�nements de changement de valeur des scores */
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        scoreHote.OnValueChanged -= OnChangementPointageHote;
        scoreClient.OnValueChanged -= OnChangementPointageClient;
    }

    /* Fonction pour augmenter le score de l'h�te
     - On incr�mente le score de l'h�te
     - On v�rifie si la partie est termin�e*/
    public void AugmenteHoteScore()
    {
        scoreHote.Value++;
        VerifieFinPartie();
    }

    /* Fonction pour augmenter le score du client
     - On incr�mente le score du client
     - On v�rifie si la partie est termin�e*/
    public void AugmenteScoreClient()
    {
        scoreClient.Value++;
        VerifieFinPartie();
    }

    // M�thode pour g�rer le changement de valeur du score de l'h�te
    // Elle est appel�e � chaque fois que le score de l'h�te change
    // Elle met � jour le texte affich� avec les scores actuels
    private void OnChangementPointageHote(int ancienScoreHote, int nouveauScoreHote)
    {
        if (ancienScoreHote == nouveauScoreHote) return; // �vite de mettre � jour si le score n'a pas chang�

        scoreTxt.text = scoreHote.Value + " - " + scoreClient.Value;
    }

    // M�thode pour g�rer le changement de valeur du score du client
    // Elle est appel�e � chaque fois que le score du client change
    // Elle met � jour le texte affich� avec les scores actuels
    private void OnChangementPointageClient(int ancienScoreClient, int nouveauScoreClient)
    {
        if (ancienScoreClient == nouveauScoreClient) return; // �vite de mettre � jour si le score n'a pas chang�

        scoreTxt.text = scoreHote.Value + " - " + scoreClient.Value;
    }


    /* Fonction pour v�rifier si la partie est termin�e
     - Si le score de l'h�te ou du client atteint le pointage cible, on affiche le panel de victoire ou de d�faite
     - On appelle la fonction GagnantHote_ClientRpc ou GagnantClient_ClientRpc selon le cas
     - On appelle la fonction FinPartie du GameManager pour terminer la partie */
    void VerifieFinPartie()
    {
        if (scoreHote.Value >= pointageCible)
        {
            GagnantHote_ClientRpc();
            GameManager.instance.FinPartie();
            
        }
        else if (scoreClient.Value >= pointageCible)
        {
            GagnantClient_ClientRpc();
            GameManager.instance.FinPartie();
            
        }
    }

    /* Fonction RPC pour afficher le panel de victoire pour l'h�te et le panel de d�faite pour le client
     - Appel�e par le serveur pour tous les clients */
    [Rpc(SendTo.Everyone)]
    private void GagnantHote_ClientRpc()
    {

        if (IsServer)
        {
            pannelVictoire.SetActive(true);
        }
        else
        {
            pannelDefaite.SetActive(true);
        }
    }
    /* Fonction RPC pour afficher le panel de victoire pour le client et le panel de d�faite pour l'h�te
     - Appel�e par le serveur pour tous les clients */
    [Rpc(SendTo.Everyone)]
    private void GagnantClient_ClientRpc()
    {

        if (IsServer)
        {
            pannelDefaite.SetActive(true);
        }
        else
        {
            pannelVictoire.SetActive(true);
        }
    }
}
