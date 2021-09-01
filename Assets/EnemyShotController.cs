using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotController : MonoBehaviour
{
    [SerializeField]
    private int bulletsAtOnce;

    [SerializeField]
    private float minAngle;
    [SerializeField]
    private float maxAngle;

    [SerializeField]
    private bool randomSpread;

    [SerializeField]
    private GameObject bulletPrefab;
    //what will we spawn?

    [SerializeField]
    private float currentFireRate;

    [SerializeField]
    private Transform origin;


    public float Shoot(){

        if(randomSpread){
            for(int i = 0; i<bulletsAtOnce; i++){
                GameObject tempbullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,transform.eulerAngles.z + Random.Range(minAngle,maxAngle)));
            }

        }else{
            if(bulletsAtOnce > 1f){
                for(int i = 0; i<bulletsAtOnce; i++){
                    var fraction = (float)i/((float)bulletsAtOnce-1f);
                    var difference = maxAngle - minAngle;
                    var fractionDiff = fraction * difference;
                    GameObject tempbullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,transform.eulerAngles.z + (fractionDiff + minAngle)));
                }
            }else{
                GameObject tempbullet = Instantiate(bulletPrefab, origin.position, Quaternion.Euler(0,0,transform.eulerAngles.z + minAngle));
            }
            //float angleStep = (maxAngle - minAngle) / ((float)bulletsAtOnce);
            //float angle = minAngle;

            //for(int i = 0; i<bulletsAtOnce + 1 ; i++){

                //Debug.Log("anglestep " + angleStep + " angle " + angle);
                //math.pi is the radius and /180 is to make it a radian. i remember sohcahtoa
                //float bulletDirectionX = transform.position.x + Mathf.Sin((angle * Mathf.PI)/ 180f);
                //float bulletDirectionY = transform.position.y + Mathf.Cos((angle * Mathf.PI)/ 180f);

                //Vector3 bulletMoveDirection = new Vector3(bulletDirectionX, bulletDirectionY,0);
                //remember vector subtraction? we are using current position to offset from.
                //Vector2 bulletFinalDirection = (bulletMoveDirection - transform.position).normalized;


                //SPAWN OR GET FROM BULLET POOL
                //set this vbullet's launch direction to this.
                //GameObject tempbullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                //tempbullet.SetActive(false); //you'd have to use a pool for this.
                //tempbullet.transform.postiion = transform.position;
                //tempbullet.transform.rotation = transform.rotation;
                //tempbullet.SetActive(true);
                //Debug.Log(bulletFinalDirection);
                //tempbullet.GetComponent<Laser>().SetMoveDirection(bulletFinalDirection);
                
                
                //tempbullet.transform.Rotate(tempbullet.transform.rotation.x,tempbullet.transform.rotation.y,tempbullet.transform.rotation.z+angle);
                //try instead of laser...an inheritance class for projectlie tbh


                //next step
                //angle += angleStep;
            //}

        }
        return Time.time + currentFireRate;
    }

    //missing a lot of getters to work nicely with the spawn reader.
    public void SetProjectilePrefab(GameObject temp){
        bulletPrefab = temp;
    }

    public void SetProjectileFireRate(float rate){
        currentFireRate = rate;
    }

    public void SetSettings(int atOnce, float min, float max, bool spread){
        bulletsAtOnce = atOnce;
        minAngle = min;
        maxAngle = max;
        randomSpread = spread;
    }

}
