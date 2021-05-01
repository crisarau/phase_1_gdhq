using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Galactic/Weapon")]
public class Weapon : ScriptableObject {
    public GameObject projectile; //the projectile type holds the damage it does.

    public float fireRate = 0.25f;

    public int baseAmmo = 15;
    public float Shoot(Vector3 pos){
        Instantiate(projectile, pos, Quaternion.identity);
        return Time.time + fireRate;
    }
    
}