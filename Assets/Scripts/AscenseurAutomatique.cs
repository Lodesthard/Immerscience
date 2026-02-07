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
        if (cibleJoueur == null)
            cibleJoueur = Camera.main.transform;

        if (porteGauche)
            posInitialeG = porteGauche.localPosition;

        if (porteDroite)
            posInitialeD = porteDroite.localPosition;
    }

    void Update()
    {
        if (cibleJoueur == null || porteGauche == null)
            return;

        float distance = Vector3.Distance(transform.position, cibleJoueur.position);

        // ZONE DE DÉTECTION INTELLIGENTE
        // La porte s'ouvre si tu es entre 1m et distanceOuverture
        // Si tu es à moins de 0.8m (donc tu as passé la porte), elle se ferme.
        bool doitEtreOuvert = (distance < distanceOuverture && distance > 0.8f);

        if (doitEtreOuvert != estOuvert)
        {
            if (doitEtreOuvert)
            {
                if (sonOuverture != null)
                {
                    sonOuverture.time = 0.3f; // Sauter le silence
                    sonOuverture.Play();
                }
            }

            estOuvert = doitEtreOuvert;
        }

        // Mouvement
        Vector3 cibleG = estOuvert ? posInitialeG + translationGauche : posInitialeG;
        Vector3 cibleD = estOuvert ? posInitialeD + translationDroite : posInitialeD;

        porteGauche.localPosition = Vector3.MoveTowards(
            porteGauche.localPosition,
            cibleG,
            vitesse * Time.deltaTime
        );

        porteDroite.localPosition = Vector3.MoveTowards(
            porteDroite.localPosition,
            cibleD,
            vitesse * Time.deltaTime
        );
    }
}
