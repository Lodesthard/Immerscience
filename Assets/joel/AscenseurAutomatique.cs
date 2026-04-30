using UnityEngine;
using System.Collections;

public class AscenseurAutomatique : MonoBehaviour
{
    [Header("Les deux battants")]
    public Transform porteGauche;
    public Transform porteDroite;

    [Header("Cibles")]
    public Transform joueur;      // Glisse la Main Camera ici
    public Transform pointArrivee; // Objet vide placé au 2ème étage

    [Header("Audio")]
    public AudioSource musiqueAttente;

    [Header("Réglages")]
    public float distanceOuverture = 3f;
    public float distanceDepartVoyage = 1.2f;
    public float vitesse = 1.5f;
    public Vector3 translationGauche = new Vector3(0, 0, -1.5f);
    public Vector3 translationDroite = new Vector3(0, 0, 1.5f);

    private Vector3 posInitialeGauche;
    private Vector3 posInitialeDroite;
    private bool voyageEnCours = false;
    private bool estOuvert = false;

    void Start()
    {
        if (porteGauche) posInitialeGauche = porteGauche.localPosition;
        if (porteDroite) posInitialeDroite = porteDroite.localPosition;
    }

    void Update()
    {
        if (joueur == null) return;

        if (voyageEnCours)
        {
            ActualiserMouvementPortes();
            return;
        }

        float distance = Vector3.Distance(transform.position, joueur.position);

        if (distance < distanceDepartVoyage)
        {
            StartCoroutine(SequenceVoyage());
            return;
        }

        estOuvert = (distance < distanceOuverture);
        ActualiserMouvementPortes();
    }

    void ActualiserMouvementPortes()
    {
        Vector3 cibleG = estOuvert ? posInitialeGauche + translationGauche : posInitialeGauche;
        Vector3 cibleD = estOuvert ? posInitialeDroite + translationDroite : posInitialeDroite;

        porteGauche.localPosition = Vector3.Lerp(porteGauche.localPosition, cibleG, Time.deltaTime * vitesse);
        porteDroite.localPosition = Vector3.Lerp(porteDroite.localPosition, cibleD, Time.deltaTime * vitesse);
    }

    IEnumerator SequenceVoyage()
    {
        voyageEnCours = true;
        estOuvert = false;

        yield return new WaitForSeconds(2.5f);

        if (musiqueAttente) musiqueAttente.Play();
        yield return new WaitForSeconds(20f);
        if (musiqueAttente) musiqueAttente.Stop();

        // --- TÉLÉPORTATION SÉCURISÉE ---
        if (pointArrivee != null && joueur != null)
        {
            // On cherche l'objet XR Origin (le parent racine du joueur)
            var xrOrigin = joueur.GetComponentInParent<Unity.XR.CoreUtils.XROrigin>();

            if (xrOrigin != null)
            {
                // 1. Déplacer le Rig complet
                xrOrigin.transform.position = pointArrivee.position;
                xrOrigin.transform.rotation = pointArrivee.rotation;

                // 2. Correction de l'Offset de la caméra (si le joueur a bougé dans sa zone réelle)
                Vector3 offsetTete = xrOrigin.Camera.transform.position - xrOrigin.transform.position;
                offsetTete.y = 0;
                xrOrigin.transform.position -= offsetTete;//xrOrigin.transform.position -= offsetTete;

                Debug.Log("Téléportation XR Origin réussie !");
            }
            else
            {
                // Secours : Déplace le parent le plus haut si XR Origin n'est pas trouvé
                joueur.root.position = pointArrivee.position;
                joueur.root.rotation = pointArrivee.rotation;
                Debug.LogWarning("XR Origin non trouvé, déplacement du Root.");
            }
        }

        yield return new WaitForSeconds(0.5f);
        estOuvert = true; // Ouverture automatique à l'arrivée
    }
}