using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Galactic/Weapon")]
public class Weapon : ScriptableObject {
    public GameObject projectile; //the projectile type holds the damage it does.
    public GameObject criticalProjectile;

    public float fireRate = 0.25f;

    public int baseAmmo = 15;
    public float Shoot(Vector3 pos, Quaternion rot, Shot criticalShot){
        if(criticalShot.critical){
            Instantiate(criticalProjectile, pos, rot);
        }else{
            Instantiate(projectile, pos, rot);
        }
        return Time.time + fireRate;
    }
    
}