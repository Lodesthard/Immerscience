using UnityEngine;

public class PorteAutomatique : MonoBehaviour
{
    public Transform porteGauche;
    public Transform porteDroite;
    public Transform joueur; // Glisse la Main Camera ici
    public float distanceOuverture = 3f; // Distance à laquelle la porte s'ouvre
    
    public Vector3 translationGauche = new Vector3(-1.5f, 0, 0);
    public Vector3 translationDroite = new Vector3(1.5f, 0, 0);
    public float vitesse = 2f;

    private Vector3 posInitialeGauche;
    private Vector3 posInitialeDroite;

    void Start()
    {
        posInitialeGauche = porteGauche.localPosition;
        posInitialeDroite = porteDroite.localPosition;
    }

    void Update()
    {
        if (joueur == null) return;

        // On calcule la distance entre le joueur et ce script
        float distance = Vector3.Distance(transform.position, joueur.position);

        // Si la distance est courte, on ouvre
        bool proche = (distance < distanceOuverture);

        Vector3 cibleG = proche ? posInitialeGauche + translationGauche : posInitialeGauche;
        Vector3 cibleD = proche ? posInitialeDroite + translationDroite : posInitialeDroite;

        porteGauche.localPosition = Vector3.Lerp(porteGauche.localPosition, cibleG, Time.deltaTime * vitesse);
        porteDroite.localPosition = Vector3.Lerp(porteDroite.localPosition, cibleD, Time.deltaTime * vitesse);
    }
}
