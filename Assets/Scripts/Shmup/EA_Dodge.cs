using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Dodge :  IEnemyEvasionManeuver
{
    private EnemyController currentEnemy;
    private Vector3 target;

    private Vector3 dirFromEnemyToBullet;
    private float dodgeSpeed;
    private float recoveryTime;
    private float secondGoChance;

    private bool haveTarget = false;

    private bool distanceReached;
    private float distanceToTravel;
    private float angle;

    private float dotFromEnemyToBullet;

    private Vector3 targetDirection;
    private Transform targetLock;

    private Collider2D currentCollisionInfo; //state depends on whether this is null or not...maybe we don't need this at all lol
    public EA_Dodge(EnemyController enemy, float speed, float distance){
        currentEnemy = enemy;
        dodgeSpeed = speed;
        distanceToTravel = distance;
    }
    public void UseEnemyAbility(){
        if(!distanceReached){
            
            currentEnemy.transform.Translate(targetDirection *  dodgeSpeed * Time.deltaTime);
            
            //we reached the distance!
            //if(Vector3.Distance(targetLock.position, currentEnemy.transform.position) < 0.2f){
            if(Time.time >= recoveryTime){
                Debug.Log("ARRIVED!");
                distanceReached = true;
                haveTarget = false;
                recoveryTime += Time.time;
            }
        }
    }

    public void SetCollisionInfo(Collider2D info){
        currentCollisionInfo = info;

        //set the target right here
        
        // ok learn about the info of the collider2D. where did it come from?
        //direction vector from where the enemy is to where the bullet came from
        dirFromEnemyToBullet = info.transform.position - currentEnemy.transform.position;
        dotFromEnemyToBullet = Vector2.Dot(dirFromEnemyToBullet.normalized, info.transform.up.normalized);
        Debug.Log("vector to bulletHit: " + dirFromEnemyToBullet + " magnitude:" + dirFromEnemyToBullet.magnitude);
        Debug.Log("dotproduct from hit: " + dotFromEnemyToBullet);
        angle = Mathf.Atan2(dirFromEnemyToBullet.y,dirFromEnemyToBullet.x) * Mathf.Rad2Deg;
        Debug.Log("angle to collision hit: " + angle);

        //THIS WORKS
        //targetDirection = Quaternion.Euler(0,0,45) * (info.transform.up.normalized * -1.0f);//dirFromEnemyToBullet;

        //ok so this is what i want...
        //if dot product - 0.9 to -1 means that we are basically heading for front hit. so make it take sides away,
        //if around 7 or 8 make it to the opposite side! what if there is a wall...who cares lol.
        if(dotFromEnemyToBullet <= -0.9f && dotFromEnemyToBullet >= -1f){

            //this is to determine random side. if 0 on right. if 1 on left
            int relativeSideRandom = Random.Range(0,2);
            if(relativeSideRandom == 0){
                relativeSideRandom = 2;
            }else{
                relativeSideRandom = 5;
            }

            targetDirection = Quaternion.Euler(0,0,45*Random.Range(relativeSideRandom,relativeSideRandom+2)) * (info.transform.up.normalized * -1.0f);
            Debug.Log("targetDirection: " +targetDirection);    
        }

        Debug.Log("angle to move the enemy on: " + Vector3.Angle(targetDirection, dirFromEnemyToBullet));

        haveTarget =true;

        recoveryTime = distanceToTravel + Time.time;
    
    }

    /*
    how to do it:
    give a ship a collider, activate it when it is on aware state...
    once it is on aware state...
        if something hits our collider...
            PROJECTILE
                Determine what direction the projectile is coming from...
                is the enemy aggresive? GO TOWARDS ENEMY. else go away from projectile...
                is there a wall? go away from it.
                make the decision. eithe way change shooting behavior.
            PLAYER...a meelee attack is coming
                GO AWAY FROM PLAYER
        
        double dodge? there is a chance that we can enter aware state again
    
    once the dodge is finished...enemy gets a chance at keeping in aware state...while continuing path
    else lets it be over.

    */

    public void ResetForReuse(EnemyController enemyReplacement){
        currentEnemy = enemyReplacement;
        haveTarget = false;
    }

    public void SetResourcesBasedOnType(EnemyAbilityResources resources){
        return;
    }
}
