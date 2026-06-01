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

    [Tooltip("Ajoute 180° au yaw du rig après téléportation (ascenseurs face à face).")]
    public bool inverserOrientation = true;

    [Tooltip("Délai (s) après une téléportation pendant lequel AUCUN ascenseur ne " +
             "peut redémarrer. Anti-boucle : le point d'arrivée peut tomber dans le " +
             "rayon d'un autre ascenseur (ou du même).")]
    public float delaiAntiBoucle = 2f;

    private Vector3 posInitialeGauche;
    private Vector3 posInitialeDroite;
    private bool voyageEnCours = false;
    private bool estOuvert = false;

    // 0 = fermé, 1 = ouvert. Progression linéaire dans le temps : l'ouverture
    // est l'exact inverse temporel de la fermeture (même vitesse, easing symétrique).
    private float ouvertureProgress = 0f;

    // Anti-boucle : le joueur doit être clairement SORTI de la zone
    // (distance > distanceOuverture) pour réarmer un départ. Empêche un
    // re-déclenchement immédiat quand le joueur est déposé dans/à côté de
    // l'ascenseur d'arrivée.
    private bool pretAuDepart = false;

    // Verrou GLOBAL partagé par tous les ascenseurs : juste après une
    // téléportation, plus aucun voyage ne peut démarrer pendant delaiAntiBoucle.
    private static float prochainDepartAutorise = 0f;

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

        // --- Réarmement anti-boucle ---
        // Pendant la fenêtre qui suit une téléportation, on DÉSARME en continu :
        // un ascenseur dans lequel le joueur vient d'être déposé ne peut pas
        // repartir tant que le joueur n'est pas ressorti.
        if (Time.time < prochainDepartAutorise)
            pretAuDepart = false;
        else if (distance > distanceOuverture)
            pretAuDepart = true;

        if (pretAuDepart && distance < distanceDepartVoyage)
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

        if (porteGauche) porteGauche.localPosition = posInitialeGauche + translationGauche * k;
        if (porteDroite) porteDroite.localPosition = posInitialeDroite + translationDroite * k;
    }

    IEnumerator SequenceVoyage()
    {
        voyageEnCours = true;
        pretAuDepart = false; // consommé : il faudra ressortir pour relancer
        estOuvert = false;

        yield return new WaitForSeconds(2.5f);

        if (musiqueAttente) musiqueAttente.Play();
        yield return new WaitForSeconds(20f);
        if (musiqueAttente) musiqueAttente.Stop();

        // --- TÉLÉPORTATION SÉCURISÉE ---
        if (pointArrivee != null && joueur != null)
        {
            // On cherche l'objet XR Origin (le parent racine du joueur)
            var xrOrigin = XRTeleportUtil.FindRig(joueur);

            if (xrOrigin != null)
            {
                XRTeleportUtil.TeleportToMarker(xrOrigin, pointArrivee, inverserOrientation, keepHeight: true);
                Debug.Log("Téléportation XR Origin réussie !");
            }
            else
            {
                // Secours : Déplace le parent le plus haut si XR Origin n'est pas trouvé
                joueur.root.position = pointArrivee.position;
                joueur.root.rotation = pointArrivee.rotation;
                Debug.LogWarning("XR Origin non trouvé, déplacement du Root.");
            }

            // Bloque TOUS les ascenseurs un court instant : le point d'arrivée
            // peut être proche d'un autre ascenseur (ou de celui-ci) -> évite la
            // téléportation en boucle.
            prochainDepartAutorise = Time.time + delaiAntiBoucle;
        }

        // Attendre que le joueur soit bien arrivé (position stabilisée)
        yield return new WaitForSeconds(0.5f);

        estOuvert = true;
        voyageEnCours = false;
        // pretAuDepart reste false : le joueur doit sortir de la zone
        // (distance > distanceOuverture) avant de pouvoir relancer cet ascenseur.
    }
}
