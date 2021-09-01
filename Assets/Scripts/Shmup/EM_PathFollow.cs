using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class EM_PathFollow : IMovementOption
{

    private EnemyController currentEnemy;
    private PathCreator pathToFollow;

    private bool rotateTowardsInput;

    private float pathSpeed;

    private float distanceTravelled;

    public EndOfPathInstruction endOfPathInstruction;

    public EM_PathFollow(EnemyController enemy, PathCreator path, float speed, bool rotation = false){
        pathToFollow = path;
        pathSpeed = speed;
        currentEnemy = enemy;
        rotateTowardsInput = rotation;
    }

    public void Move(){
        Debug.Log("FOLLOWING PATH");
        distanceTravelled+= pathSpeed*Time.deltaTime;
        currentEnemy.transform.position = pathToFollow.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        Debug.Log(pathToFollow.path.GetRotationAtDistance(distanceTravelled,endOfPathInstruction));
            
        if(rotateTowardsInput){
            //Debug.Log(pathToFollow.path.GetRotationAtDistance(distanceTravelled,endOfPathInstruction));
            //currentEnemy.transform.rotation = pathToFollow.path.GetRotationAtDistance(distanceTravelled,endOfPathInstruction);
            currentEnemy.transform.rotation = Quaternion.LookRotation(Vector3.forward, pathToFollow.path.GetDirectionAtDistance(distanceTravelled, endOfPathInstruction) * -1);
             
        }
    }

    public void SetResourcesBasedOnType(EnemyMovementResources res){

        rotateTowardsInput = res.firstOption;
        GameObject tempPathObj = Object.Instantiate(res.pathObject);
        tempPathObj.gameObject.transform.position =  res.pathPlacement;
        SetPath(tempPathObj.GetComponent<PathCreator>());
    }

    public void SetPath(PathCreator path){
        pathToFollow = path;
    }

    public void ResetForReuse(EnemyController enemyReplacement){
        distanceTravelled = 0;
        currentEnemy = enemyReplacement;
        pathSpeed = enemyReplacement.GetSpeed() * 2; //change this lol
    }
}
