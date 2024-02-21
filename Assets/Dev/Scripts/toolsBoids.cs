using UnityEngine;
using static CreateBoidStruct;

[System.Serializable]
public class toolsBoids : MonoBehaviour
{

    // Sommaire :
    // 1. Initialisation du tableau ainsi que de leur position dans le monde
    // 2. M�thodes de comportement des boids

    [Header("Parameters Boids")]
    [SerializeField] private Boid[] boids; // Tableau de struct Boid qui contient tous ses membres
    [SerializeField] private int NumberBoids = 10; // Nombre de boid dans le tableau
    [SerializeField] private int NumberChief = 5; // Tous les "NumberChief" ajoute un chef dans le tableau. Par exemple tous les 5 du tableau tu me rajoutes un chef.
    [SerializeField] private float speed = 0.2f; // La vitesse d'un boid
    [Range(1, 60)][SerializeField] private float radiusCohesion = 12; // La taille de la zone de coh�sion
    private float speedRotation = 3.5f; // La speedRotation, est proportionnelle � la speed
    private int chief; // Repr�sente le nombre de chef dans le tableau

    [Header("Collision")]
    [Range(1, 100)][SerializeField] private float forceCollision; // Repr�sente la valeur de distance lorsque qu'un boids collisionne un autre boid

    [Header("Ennemy Collision")]
    [Range(1, 300)][SerializeField] private float forceCollisionEnnemy; // Represente la valeur de distance lorsque qu'un boid collisionne un pr�dateur

    [Header("Debug Time")]
    public float time = 1; // Sert de debug pour aller plus vite dans le temps
    [SerializeField] private bool ChangeColorChiefInStartMethode; // Permets au d�marrage de la sc�ne d'autoriser le changement de couleur des chefs
    private Renderer material_renderer;

    [Header("Parameters Generals")]
    [SerializeField] private GameObject visual_prefab; // Choisi le visual
    [SerializeField] private GameObject zone; // La zone g�n�rale ou la zone de coh�sion va spwan
    [SerializeField] private GameObject boidsArrayDossier; // Force les utilisateurs � ajouter un dossier qui regroupe tous les boids sinon devient un giga bordel dans la hi�rarchie
                                                           // Je pourrais mettre une s�curit� mais je veux que �a soit obligatoire sinon c'est relou !!!!
    // Start is called before the first frame update
    void Start()
    {
        InitBoidArray();
        SpawnBoids();
        WhoIsChef(0);
        if(ChangeColorChiefInStartMethode) Invoke("ColorChief", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Proportion();
        Cohesion();
        Alignement();

        Time.timeScale = time;

        for (int i = 0; i < NumberBoids; i++)
        {
            // V�rifie si le boid actuel n'est pas un chef
            if (boids[i].IsChief == false)
            {
                // V�rifie les collisions pour ce boid
                boids[i].CheckCollision(forceCollision, forceCollisionEnnemy);
            }

            // Fait avancer le boid avec la vitesse et la vitesse de rotation sp�cifi�es
            boids[i].Move(speed , speedRotation);
        }
    }
    /// <summary>
    /// // 1. Initialisation du tableau ainsi que de leur position dans le monde
    /// </summary>
    /// 

    // Initialise le tableau de boids
    void InitBoidArray()
    {
        boids = new Boid[NumberBoids];
    }

    // Cr�e les boids visuels dans la zone sp�cifi�e
    void SpawnBoids()
    {
        for (int i = 0; i < NumberBoids; i++)
        {
            // Instancie un boid visuel � une position al�atoire dans la zone sp�cifi�e
            boids[i].visualGo = Instantiate(visual_prefab, SpawnInZone(), Quaternion.identity , boidsArrayDossier.transform);
            // Nomme le boid
            boids[i].visualGo.name = "Boid " + i ;
            // D�finit la destination initiale du boid comme �tant sa position actuelle
            boids[i].destination = boids[i].visualGo.transform.position;
        }
    }
    // G�n�re un point de spawn al�atoire dans la zone d�finie
    Vector3 SpawnInZone()
    {
        // G�n�re une position al�atoire dans une zone cubique d�finie
        Vector3 _spawnZone = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        // Transforme la position dans le rep�re local de la zone en position dans le rep�re global
        _spawnZone = zone.transform.TransformPoint(_spawnZone);

        // Retourne la position g�n�r�e
        return _spawnZone;
    }

    // D�termine quel boid est le chef
    void WhoIsChef(int count)
    {
        for (int i = 0; i < boids.Length; i++)
        {
            // Incr�mente le compteur
            count++;
            // Si le compteur atteint le nombre de chefs d�sir�
            if (count == NumberChief)
            {
                // Marque le boid actuel comme chef
                boids[i].IsChief = true;
                // R�initialise le compteur
                count = 0; 
            }
        }
    }

    // Ajoute une couleur al�atoire aux chefs
    void ColorChief()
    {
        for (int i = 0; i < NumberBoids; i++)
        {
            // Si le boid est un chef
            if (boids[i].IsChief == true)
            {
                // R�cup�re le composant Renderer du boid visuel
                material_renderer = boids[i].visualGo.GetComponentInChildren<Renderer>();
                // D�finit une couleur al�atoire pour le chef
                material_renderer.material.SetColor("_Color", new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255)) * 0.008f);
            }
        }
    }
    /// <summary>
    /// // END 1. Initialisation du tableau ainsi que de leur position dans le monde
    /// </summary>
    /// 



    /// <summary>
    /// // 2. M�thodes de comportement des boids
    /// </summary>
    ///
    

    // Ajuste la proportion de la vitesse de rotation
    void Proportion()
    {
        speedRotation = (speed * speedRotation) / speed;
    }
    // G�re le comportement de coh�sion des boids, cette fonction va g�rer la destination des boids chef
    void Cohesion()
    {
        for (int i = 0; i < NumberBoids; i++)
        {
            // V�rifie si le boid actuel est un chef et s'il est proche de sa destination
            if (boids[i].IsChief == true && Vector3.Distance(boids[i].visualGo.transform.position , boids[i].destination) < 2.5f)
            {
                // R�oriente la destination du chef vers un point al�atoire � l'int�rieur de la zone de coh�sion
                boids[i].destination = zone.transform.position + Random.insideUnitSphere * radiusCohesion ;
            }
        }
    }

    // G�re le comportement d'alignement des boids, cette fonction va g�rer la destination des boids non chef
    void Alignement()
     {
        for (int i = 0; i < NumberBoids; i++)
        {
            // Si le boid actuel est un chef, d�termine l'index du chef
            if (boids[i].IsChief)
            {
                chief = i; 
            }

            // Si le boid n'est pas un chef
            if (!boids[i].IsChief)
            {
                // Ajuste la destination du boid vers la destination du chef en utilisant une interpolation lin�aire
                boids[i].destination = Vector3.Lerp(boids[i].destination, boids[chief].destination, Time.deltaTime);
            }
        }
    }
    /// <summary>
    /// // END 2. M�thodes de comportement des boids
    /// </summary>
    /// 

    /* public void OnDrawGizmos()
      {
          Gizmos.color = Color.red;
          Gizmos.DrawWireSphere(zone.transform.position, radiusCohesion);
          if (boids.Length > 0)
          {
              for (int i = 0; i < NumberBoids; i++)
              {
                  if (boids[i].IsChief == true)
                  {
                      //Gizmos.color = Color.red;
                      //Gizmos.DrawWireSphere(boids[i].visualGo.transform.position, 1);
                      Gizmos.color = Color.green;
                      Gizmos.DrawWireSphere(boids[i].destination, 0.5f);
                  }
              }

          }
      }*/
}
