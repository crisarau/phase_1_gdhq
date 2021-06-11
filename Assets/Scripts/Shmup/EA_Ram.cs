using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ram : IEnemyAbility
{

    private float chargeTime;
    private float ramSpeed;
    private float recoveryTime;
    private Vector3 target;
    private Vector3 dir;
    private Transform targetplayer;
    private Transform targetLock;
    private Enemy currentEnemy;

    private bool distanceReached;
    private float distanceToTravel;
    private float angle;

    public EA_Ram(Enemy enemy, float timeToCharge, float speed, float timeToRecover, float distance, Transform player, Transform locktarget){
        currentEnemy = enemy;
        chargeTime = Time.time + timeToCharge;
        recoveryTime = timeToRecover;
        ramSpeed = speed;
        distanceReached = false;
        targetplayer = player;
        targetLock = locktarget;
        distanceToTravel = distance;
    }
    public void UseEnemyAbility(){
        if(target == Vector3.zero){
            Aim();
            //if(target!=Vector3.zero){
            //    recoveryTime += Time.time;
            //}
        }else if(distanceReached == false) {

            currentEnemy.transform.Translate(target *  ramSpeed * Time.deltaTime);

            if(Vector3.Distance(targetLock.position, currentEnemy.transform.position) < 0.2f){
                Debug.Log("ARRIVED!");
                distanceReached = true;
                recoveryTime += Time.time;
            }
        }else{
            if(Time.time >= recoveryTime){
                //NOT DOING ANYTHING
                currentEnemy.abilityActive = false;
                //RESET EVERYTHING for next time...
            }
        }
    }
    public void Aim(){
        if(Time.time <= chargeTime){
            //aim and rotate according to player position. you get an angle.
            targetLock.position = targetplayer.position;
            dir = targetLock.position - currentEnemy.transform.position;
            Debug.Log("vector to player: " + dir + " magnitude:" + dir.magnitude);
            angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            Debug.Log("angle to player: " + angle);
        }else{
            //use this angle we got and make a transform based on the distance we want from it...place the target here...
            //var rot = Quaternion.AngleAxis(45,Vector3.right);
            //var lDirection = angle * Vector3.forward;

            dir = Vector3.Normalize(dir);
            target = dir * distanceToTravel;
            
            //Debug.Log("target hitting: " + target + " magnitude:" + target.magnitude);
            //targetplayer.position = target;
            //Debug.Log("target hitting: " + target + " magnitude:" + target.magnitude);
            targetLock.position = targetLock.TransformDirection(target) + currentEnemy.transform.position; //offset by the object relative to it
            //Debug.Log("target hitting: " + targetplayer.position); //+ " magnitude:" + targetplayer.position.magnitude);
        }
    }
}
