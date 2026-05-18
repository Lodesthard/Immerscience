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

    // 0 = fermé, 1 = ouvert. Progression linéaire dans le temps : l'ouverture
    // est l'exact inverse temporel de la fermeture (même vitesse, easing symétrique).
    private float ouvertureProgress = 0f;

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

        if (distance < distanceDepartVoyage && !voyageEnCours)
        {
            StartCoroutine(SequenceVoyage());
            return;
        }

        if (!estOuvert && distance < distanceOuverture)
            estOuvert = true;
        else if (estOuvert && distance > distanceOuverture + 1.5f)
            estOuvert = false;
        ActualiserMouvementPortes();
    }

    void ActualiserMouvementPortes()
    {
        // Progression linéaire à vitesse constante vers la cible (1=ouvert, 0=fermé).
        // MoveTowards => même durée à l'ouverture qu'à la fermeture.
        float cible = estOuvert ? 1f : 0f;
        ouvertureProgress = Mathf.MoveTowards(ouvertureProgress, cible, Time.deltaTime * vitesse);

        // SmoothStep est symétrique : SmoothStep(1-p) == 1-SmoothStep(p).
        // => la courbe d'ouverture est l'exact miroir temporel de la fermeture.
        float k = Mathf.SmoothStep(0f, 1f, ouvertureProgress);

        porteGauche.localPosition = posInitialeGauche + translationGauche * k;
        porteDroite.localPosition = posInitialeDroite + translationDroite * k;
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

        // Attendre que le joueur soit bien arrivé (position stabilisée)
        yield return new WaitForSeconds(0.5f);

        estOuvert = true;
        voyageEnCours = false;
    }
}