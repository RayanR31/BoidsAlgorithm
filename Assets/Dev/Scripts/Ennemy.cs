using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    /// <summary>
    /// Code non pr�t pour faire un vrai pr�dateur
    /// </summary>
    public float forceX; 
    public float forceY; 
    public float forceZ; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3 (forceX, forceY, forceZ) * Time.deltaTime;
    }
}
