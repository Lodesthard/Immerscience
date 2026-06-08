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

    [Header("Délai minimum dans la salle")]
    [Tooltip("Temps minimum (s) que le joueur doit passer HORS de l'ascenseur (= dans " +
             "la salle) avant de pouvoir relancer le voyage. Le chrono démarre quand le " +
             "joueur quitte la zone de l'ascenseur après son arrivée. 0 = désactivé.")]
    public float tempsMinimumDansSalle = 0f;

    [Header("Ouverture des portes de l'ascenseur d'arrivée")]
    [Tooltip("Ascenseur situé au point d'arrivée. À la fin de la téléportation, ses " +
             "portes s'ouvrent automatiquement (le joueur vient d'y être déposé). " +
             "Laisser vide si aucun.")]
    public AscenseurAutomatique ascenseurDestination;

    [Tooltip("Durée (s) pendant laquelle les portes restent forcées ouvertes après une " +
             "ouverture déclenchée par la téléportation, le temps que le joueur sorte. " +
             "Empêche la détection de proximité de les refermer aussitôt.")]
    public float dureeForcerOuverture = 6f;

    // Tant que Time.time < cette valeur, estOuvert est forcé à true (ouverture
    // déclenchée par l'arrivée d'une téléportation, indépendante de la proximité).
    private float forcerOuvertureJusqu = 0f;

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

    // Moment où le joueur est sorti de la zone de l'ascenseur (= est entré dans
    // la salle). Sert au gate `tempsMinimumDansSalle`. -1 = pas encore sorti.
    private float tempsSortieZone = -1f;

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
        {
            // Premier instant où le joueur quitte la zone après son arrivée :
            // démarre le chrono de présence dans la salle.
            if (!pretAuDepart && tempsSortieZone < 0f)
                tempsSortieZone = Time.time;
            pretAuDepart = true;
        }

        // Gate : le joueur doit avoir passé `tempsMinimumDansSalle` secondes hors
        // de l'ascenseur (dans la salle) avant de pouvoir relancer un voyage.
        bool delaiSalleOk = tempsMinimumDansSalle <= 0f
            || (tempsSortieZone >= 0f && Time.time - tempsSortieZone >= tempsMinimumDansSalle);

        if (pretAuDepart && delaiSalleOk && distance < distanceDepartVoyage)
        {
            StartCoroutine(SequenceVoyage());
            return;
        }

        if (!estOuvert && distance < distanceOuverture)
            estOuvert = true;
        else if (estOuvert && distance > distanceOuverture + 1.5f)
            estOuvert = false;

        // Ouverture forcée suite à une téléportation : prioritaire sur la proximité.
        if (Time.time < forcerOuvertureJusqu)
            estOuvert = true;

        ActualiserMouvementPortes();
    }

    // Ouvre les portes immédiatement (et les maintient ouvertes un court instant).
    // Appelé par l'ascenseur de départ juste après avoir téléporté le joueur ici.
    public void DemanderOuverture()
    {
        forcerOuvertureJusqu = Time.time + dureeForcerOuverture;
        estOuvert = true;
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
        tempsSortieZone = -1f; // ré-arme le chrono de présence dans la salle
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

            // Ouvre les portes de l'ascenseur où le joueur vient d'être déposé.
            if (ascenseurDestination != null)
                ascenseurDestination.DemanderOuverture();
        }

        // Attendre que le joueur soit bien arrivé (position stabilisée)
        yield return new WaitForSeconds(0.5f);

        estOuvert = true;
        voyageEnCours = false;
        // pretAuDepart reste false : le joueur doit sortir de la zone
        // (distance > distanceOuverture) avant de pouvoir relancer cet ascenseur.
    }
}
