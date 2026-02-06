using UnityEngine;

public class AscenseurEntreeUniquement : MonoBehaviour
{
    [Header("Les deux battants")]
    public Transform porteGauche;
    public Transform porteDroite;
    
    [Header("Composants")]
    public AudioSource sonOuverture; // Le son qui se joue à l'approche
    public Transform cibleJoueur;
    
    [Header("Réglages")]
    public float distanceOuverture = 5f;
    public Vector3 translationGauche = new Vector3(0, 1.2f, 0);
    public Vector3 translationDroite = new Vector3(0, -1.2f, 0);
    public float vitesse = 2f;

    private Vector3 posInitialeG;
    private Vector3 posInitialeD;
    private bool estOuvert = false;

    void Start()
    {
        if (cibleJoueur == null) cibleJoueur = Camera.main.transform;
        
        if (porteGauche) posInitialeG = porteGauche.localPosition;
        if (porteDroite) posInitialeD = porteDroite.localPosition;
    }

    void Update()
    {
        if (cibleJoueur == null || porteGauche == null || porteDroite == null) return;

        float distance = Vector3.Distance(transform.position, cibleJoueur.position);
        bool doitEtreOuvert = (distance < distanceOuverture);

        // LOGIQUE DE CHANGEMENT D'ÉTAT
        if (doitEtreOuvert != estOuvert)
        {
            sonOuverture.time = 0.7f; // SAUTE les 0.5 premières secondes de silence
            // Si on vient de franchir le seuil pour OUVRIR
            if (doitEtreOuvert == true) 
            {
                if (sonOuverture != null)
                {
                    sonOuverture.Play();
                    Debug.Log("Son d'ouverture activé !");
                }
            }
            // Si on vient de franchir le seuil pour FERMER
            else 
            {
                Debug.Log("Fermeture silencieuse (éloignement).");
            }

            estOuvert = doitEtreOuvert;
        }

        // Mouvement des portes
        Vector3 cibleG = estOuvert ? posInitialeG + translationGauche : posInitialeG;
        Vector3 cibleD = estOuvert ? posInitialeD + translationDroite : posInitialeD;

        porteGauche.localPosition = Vector3.Lerp(porteGauche.localPosition, cibleG, Time.deltaTime * vitesse);
        porteDroite.localPosition = Vector3.Lerp(porteDroite.localPosition, cibleD, Time.deltaTime * vitesse);
    }
}

