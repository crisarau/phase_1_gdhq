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
    private Vector3 startingPosition;
    private Transform targetplayer;
    private EnemyController currentEnemy;

    private bool distanceReached;
    private float distanceToTravel;
    private float angle;
    private bool haveTarget;

    private bool rotateTowardsTarget;

    public EA_Ram(EnemyController enemy, float timeToCharge, float speed, float timeToRecover, float distance, Transform player){
        currentEnemy = enemy;
        chargeTime = Time.time + timeToCharge;
        recoveryTime = timeToRecover;
        ramSpeed = speed;
        distanceReached = false;
        targetplayer = player;
        distanceToTravel = distance;
    }
    public void UseEnemyAbility(){
        if(haveTarget == false){
            Debug.Log("don't have target...AIM!");
            Aim();
            //if(target!=Vector3.zero){
            //    recoveryTime += Time.time;
            //}
        }else if(distanceReached == false) {

            Debug.Log("moving towards target");
            if(!rotateTowardsTarget){
                currentEnemy.transform.Translate(target *  ramSpeed * Time.deltaTime);
            }else{
                //uncomment if you want a littele of a follow
                //currentEnemy.transform.rotation = Quaternion.RotateTowards(currentEnemy.transform.rotation, Quaternion.LookRotation(Vector3.forward, -(targetplayer.position - currentEnemy.transform.position)), Time.deltaTime * 180f);
                currentEnemy.transform.Translate(-Vector3.up *  ramSpeed * Time.deltaTime);
            }
            
            if(Vector3.Distance(currentEnemy.transform.position, startingPosition) >= distanceToTravel){
                Debug.Log("ARRIVED!");
                distanceReached = true;
                recoveryTime += Time.time;
            }
        }else{
            if(Time.time >= recoveryTime){
                Debug.Log("we done for real");
                //NOT DOING ANYTHING
                currentEnemy.SetAbilityStatus(false);
                currentEnemy.SetAbilityBoundaries(1.5f, 3.5f);
                currentEnemy.RollForNextAbilityLaunch();
                //RESET EVERYTHING for next time...
            }
        }
    }
    public void Aim(){
        if(Time.time <= chargeTime){
            //aim and rotate according to player position. you get an angle.
            //targetLock.position = targetplayer.position;
            //dir = targetLock.position - currentEnemy.transform.position;
            
            dir = targetplayer.position - currentEnemy.transform.position;
            
            if(rotateTowardsTarget){
                currentEnemy.transform.rotation = Quaternion.RotateTowards(currentEnemy.transform.rotation, Quaternion.LookRotation(Vector3.forward, -dir), Time.deltaTime * 180f);
            }

            Debug.Log("vector to player: " + dir + " magnitude:" + dir.magnitude);
            angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            Debug.Log("angle to player: " + angle);

            Debug.Log("charging");
        }else{
            dir = targetplayer.position - currentEnemy.transform.position;
            dir = Vector3.Normalize(dir);
            target = dir*distanceToTravel;
            //target += currentEnemy.transform.position;
            Debug.Log("enemy is at: " + currentEnemy.transform.position);
            startingPosition = currentEnemy.transform.position;
            Debug.Log("DECIDED ON TARGET " + target);
            haveTarget = true;


            //changing shoot behavior...based on aggro?
            currentEnemy.SetConsecutiveShots(6);
            currentEnemy.Shoot();

        }
    }

    public void ResetForReuse(EnemyController enemyReplacement){
        currentEnemy = enemyReplacement;
        ramSpeed = currentEnemy.GetSpeed() * 1.5f;
        distanceReached = false;
        haveTarget = false;
        chargeTime = Time.time + 1f;
        recoveryTime = 0.5f;
        distanceToTravel = 4.5f;

        rotateTowardsTarget = false;
    }

    public void SetResourcesBasedOnType(EnemyAbilityResources resources){
        targetplayer = resources.liveTransformTarget;
    }
}
