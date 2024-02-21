using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CreateBoidStruct : MonoBehaviour
{
    // Sommaire :
    // 1. Définition de la structure Boid
    // 2. Méthode de déplacement des boids
    // 3. Méthode de vérification des collisions
    [Serializable]public struct Boid
    {
        /// <summary>
        /// 1. Définition de la structure Boid
        /// </summary>
        public GameObject visualGo; // Référence au GameObject visuel du boid
        public bool IsChief; // Indique si le boid est un chef
        public Vector3 destination; // Destination vers laquelle le boid se déplace

        /// <summary>
        /// 2. Méthode de déplacement des boids
        /// </summary>
        /// Déplace le boid vers sa destination tout on la regardant. 
        /// La destination des chefs est un point aléatoire dans une zone de cohésion prédéfinie.
        /// La destination des boids classiques est la destination de leur chef
        public void Move(float _speed , float _speedRotation)
        {
            // Déplacement du boid vers sa destination avec une vitesse donnée
            visualGo.transform.position += (destination - visualGo.transform.position) * Time.deltaTime * _speed;

            // Rotation du boid pour regarder sa destination
            visualGo.transform.rotation = Quaternion.Lerp(visualGo.transform.rotation, Quaternion.LookRotation(destination - visualGo.transform.position), Time.deltaTime * _speedRotation);
        }

        /// <summary>
        /// 3. Méthode de vérification des collisions
        /// </summary>
        /// Tir des sphereCast tout autour du joueur pour vérifier si il y a un obsatacle, si oui alors le boid se réajuste en fonction de la hit.normal et d'une distance minimum
        public void CheckCollision(float _forceCollision , float _forceCollisionPredateur)
        {
            // Directions de recherche pour la détection des collisions
            Vector3[] directions = { Vector3.up, -Vector3.up, Vector3.right, Vector3.left, Vector3.forward, -Vector3.forward };

            // Parcours de chaque direction
            foreach (Vector3 directionBoucleEach in directions)
            {
                RaycastHit hit;

                // Vérifie s'il y a une collision dans la direction actuelle
                if (Physics.SphereCast(visualGo.transform.position, 0.5f, Quaternion.LookRotation(destination - visualGo.transform.position) * directionBoucleEach, out hit, 1f))
                {
                    // Si la collision est avec un prédateur
                    if (hit.transform.gameObject.layer == 6)
                    {

                        // Ajuste la destination du boid en fonction de la collision avec le prédateur
                        destination += (hit.normal * _forceCollisionPredateur * Time.deltaTime); // 15 valeur ok && 1 de distance
                                                                                                //Debug.DrawLine(_boidGo.transform.position, hit.point, Color.red);
                    }

                    // Si la collision est avec un obstacle
                    if (hit.transform.gameObject.layer == 1 >> 6)
                    {
                        // Ajuste la destination du boid en fonction de la collision avec l'obstacle
                        destination += (hit.normal * _forceCollision * Time.deltaTime); // 15 valeur ok && 1 de distance
                                                                                        //Debug.DrawLine(_boidGo.transform.position, hit.point, Color.red);
                    }
                }
                else
                {
                    //Debug.DrawRay(_boidGo.transform.position, Quaternion.LookRotation(direction) * _direction * 1f, Color.green);
                }
            }
        }
    }
}
