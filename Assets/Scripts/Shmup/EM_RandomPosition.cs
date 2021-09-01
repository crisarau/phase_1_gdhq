using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM_RandomPosition : IMovementOption
{
    private Transform currentTarget;

    private Vector3 currentTargetPosition;
    private Transform currentPlayer;
    private bool setRandomTarget;
    private bool lockX;
    private bool lockY;
    private bool rotateTowardsInput;

    private float destinationRange;

    private EnemyController currentEnemy;
    
    //testing the random finding target logic
    //spawn this thing to see where it is.
    //private GameObject targetIndicator;
    private bool haveTarget;
    private bool outOfBoundsCarryOver;
    private Vector3 outOfBoundsCarryOverDirection;
    private float arrivedThreshold = 1f;
    private float outOfBoundCheckDelay = 0f;
    Vector3 direction;

    private float speed; // speed obtained from stats of character

    public EM_RandomPosition(EnemyController enemy, Transform target, Transform playerRef, bool setRandom,float initialSpeed, bool rotation = false){
        currentTarget = target;
        
        currentPlayer = playerRef;
        setRandomTarget = setRandom;
        currentEnemy = enemy;
        speed = initialSpeed;
        rotateTowardsInput = rotation;

        //targetIndicator = indicator;

        haveTarget = true;

        //FindNewTarget(currentPlayer.position - currentEnemy.transform.position);
    }
    public void Move()
    {
        //Debug.Log("current target is at: " +  currentTargetPosition + " magnitude from enemy to position: " + direction.magnitude);


        //what to do if we are selecting a target randomly?
        //if we on have target...
        if(haveTarget){

            direction = GetDistanceToCurrentTarget();

            //move towards target
            MoveToTarget(direction);
            //are we on a timer to give it time to escape the bounds zones? do we check or not? most often we do check
            if(Time.time > outOfBoundCheckDelay){
            //are we out of bounds? now we check
                if(OutOfBoundsX() || OutOfBoundsY()){
                    //save into a bool that we have a new target to carry over from previous direction. out of bounds case
                    outOfBoundsCarryOver = true;
                    haveTarget = false;

                    return;
                }
            }
            //did we arrive?
            if(direction.magnitude <= arrivedThreshold){
                //we arrived!
                //Debug.Log("WE ARRIVED AT TARGET...SELECTING NEW TARGET");
                //select a new target by changing bool
                haveTarget = false;
            }
                
        }else{
            //check if out of bounds use case, the enemy controller will natureally take care of that, we just have to make a new target in place.
            //note that this check happens over the assumption that moveoption is called before teh enemycontroller check and corrects for it.
            if(outOfBoundsCarryOver){
                //use the direction saved from previous cycle to figure out where to go.
                //once new target found...return for next call.
                outOfBoundsCarryOver= false;
                outOfBoundCheckDelay = Time.time + 1f;
                haveTarget = true;
                return;
            }

            //direction = GetDistanceToCurrentTarget();
            //select a new target
            FindNewTarget(currentPlayer.position - currentEnemy.transform.position);
            haveTarget = true;
        }
    }

    //caps a target if too far off
    private void OutOfBoundsYCap(){
        
        //if(currentEnemy.transform.position.y >= 7f){
        if(currentTargetPosition.y >= 7f){
            //Debug.Log("Out of bounds Y find target in other direction plz"); //how about a situation when we out of bounds but need to get back somehow...start a timer on which to check if still out of bounds
            //or how about we change the transform to not go out of bounds in the first place lol it should resolve itself like that lol, but no cool out of screen interaction
            //let;s make that the case for up and down and see what happens.
            //currentTarget.transform.position = new Vector3(currentEnemy.transform.position.x,8f,0f);
            currentTargetPosition.y = 7f;
        //}else if(currentEnemy.transform.position.y <= -7f){
        }else if(currentTargetPosition.y <= -7){
            //currentTarget.transform.position = new Vector3(currentEnemy.transform.position.x,-8f,0f) ;
            currentTargetPosition.y = -7f;
        }
    }
    //makes it go to the opposite direction to get back into battle field on X axis
    private bool OutOfBoundsX(){
        
        float selectionRanges = 90f; //needs some angle range for x with no rotation otherwise...it doesn't work for some reason
        Vector3 finalDecision = new Vector3();
        float distanceTravel = 8f;
        if(currentEnemy.transform.position.x >= 6f){
            Debug.Log("WE ARE OUT OF X BOUNDS!");
            
            finalDecision = (Quaternion.AngleAxis(Random.Range(-selectionRanges, selectionRanges), Vector3.forward) * ( (-Vector2.right.normalized * Random.Range(6f, distanceTravel))));
            //GameObject temp = Object.Instantiate(targetIndicator, currentEnemy.transform.position + finalDecision, Quaternion.identity);
            //Object.Destroy(temp, 0.5f);
            //currentTarget.position = currentEnemy.transform.position + finalDecision;
            currentTargetPosition = currentEnemy.transform.position + finalDecision;
            Debug.Log("NEW TARGET ACQUIRED BY X OUT OF BOUND" + currentTargetPosition);

            return true;
        }else if(currentEnemy.transform.position.x <= -6f){
            Debug.Log("WE ARE OUT OF X BOUNDS!");
            finalDecision = (Quaternion.AngleAxis(Random.Range(-selectionRanges, selectionRanges), Vector3.forward) * ( (Vector2.right.normalized * Random.Range(6f, distanceTravel))));
            //GameObject temp = Object.Instantiate(targetIndicator, currentEnemy.transform.position + finalDecision, Quaternion.identity);
            //Object.Destroy(temp, 0.5f);
            //currentTarget.position = currentEnemy.transform.position + finalDecision;
            currentTargetPosition = currentEnemy.transform.position + finalDecision;
            Debug.Log("NEW TARGET ACQUIRED BY X OUT OF BOUND" + currentTargetPosition);
            return true;
        }
        return false;
    }

    private bool OutOfBoundsY(){
        float selectionRanges = 45f; 
        Vector3 finalDecision = new Vector3();
        float distanceTravel = 8f;
        if(currentEnemy.transform.position.y >= 6.7f){
            finalDecision = (Quaternion.AngleAxis(Random.Range(-selectionRanges, selectionRanges), Vector3.forward) * ( (-Vector2.up.normalized * Random.Range(6f, distanceTravel))));
            currentTargetPosition = currentEnemy.transform.position + finalDecision;
            OutOfBoundsYCap();
            return true;
            
        }else if(currentEnemy.transform.position.y <= -6.7f){
            finalDecision = (Quaternion.AngleAxis(Random.Range(-selectionRanges, selectionRanges), Vector3.forward) * ( (Vector2.up.normalized * Random.Range(6f, distanceTravel))));
            currentTargetPosition = currentEnemy.transform.position + finalDecision;
            OutOfBoundsYCap();
            return true;
        }
        return false;
    }

    private Vector3 GetDistanceToCurrentTarget(){
        //return currentTarget.position - currentEnemy.transform.position;
        Debug.Log("distance from currentTarget " + currentTargetPosition + " to " + " enemy is " + currentEnemy.transform.position + " with " + (currentTargetPosition - currentEnemy.transform.position).magnitude + "magnitude");
        //Debug.Log(currentEnemy.transform.TransformPoint(currentEnemy.transform.position));
        //Debug.Log("distance from currentTarget " + currentTargetPosition + " to " + " enemy is " + currentEnemy.transform.TransformPoint(currentEnemy.transform.position) + " with " + (currentTargetPosition - currentEnemy.transform.TransformPoint(currentEnemy.transform.position)).magnitude + "magnitude");
        //the usual position is the correct world one no need to do the thing above
        return currentTargetPosition - currentEnemy.transform.position;
    }

    private void MoveToTarget(Vector3 dir){
        //Vector3 dir = currentTarget.position - currentEnemy.transform.position;
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg; //the problem i think is that it's angle against world coordinates
        Debug.Log("angle: "+ angle);
        
        if(rotateTowardsInput){
            
        
            //i think i know what to do... https://forum.unity.com/threads/rotating-a-vector-by-an-eular-angle.18485/
            //use a different logic over all for translation if we rotating...:3 based on angle... because this is clearly not working because of the predetermined direction we are making
            currentEnemy.transform.rotation = Quaternion.RotateTowards(currentEnemy.transform.rotation, Quaternion.LookRotation(Vector3.forward, -dir), Time.deltaTime * 180f);
            currentEnemy.transform.Translate(-Vector3.up * speed * Time.deltaTime);
            //this works because it is relative lol and because before we were going in a direction that was relative to world space...

            //currentEnemy.transform.Translate(getInputFromAngle(angle) *  speed * Time.deltaTime, Space.World); //this works too but not as smooth lol actually takes angle into account!

        }else{
            //keep moving towards the current transform target
            currentEnemy.transform.Translate(getInputFromAngle(angle) *  speed * Time.deltaTime);

        }
    }

    public void FindNewTarget(Vector3 dir){

        //create a vector at an angle away from target
        //Vector3 dir = currentTarget.position - currentEnemy.transform.position;
        //Debug.Log(dir.magnitude);//displays current magnitude. remember that it is always possitive
        
        Vector3 finalDecision = new Vector3();
        //ask for angle against world down vector
        float angle = Vector2.Angle(-Vector2.up, dir);
        Debug.Log("world angle against direction" + angle);
        Debug.Log("world angle against enemyup" + Vector2.Angle(-currentEnemy.transform.up, dir));
        //it goes from 0 to 180 on both sides. so no difference in the left and right...so we just have to do 180 + (180 - x) lol
        float aggro = currentEnemy.GetCurrentAgression();
        float selectionRanges = 180f; // consider changing to 90 if the situation calls for it! 
        float distanceTravel = 8f;

        if(aggro <= 0.5f){
            //minus dir for away
            finalDecision = (Quaternion.AngleAxis(Random.Range(-selectionRanges, selectionRanges), Vector3.forward) * ( (-dir.normalized * Random.Range(3f, distanceTravel))));
            Debug.Log("randomly chosen direction " + Vector2.Angle(-Vector2.up, finalDecision));
            
        }else{
            //towards
            finalDecision = (Quaternion.AngleAxis(Random.Range(-selectionRanges, selectionRanges), Vector3.forward) * ( (dir.normalized * Random.Range(3f, distanceTravel))));
            Debug.Log("randomly chosen direction " + Vector2.Angle(-Vector2.up, finalDecision));
        }

        

        ///so what this does is enemy position + rotated normal at this position... WORLD SPACE BECAUSE A DIRECTION IS ALWAYS THE SAME. note that this direction that we add on angle axis is relative to what the direction vector was
        //GameObject temp = Object.Instantiate(targetIndicator, currentEnemy.transform.position + finalDecision, Quaternion.identity);
        //Object.Destroy(temp,0.5f);
        //currentTarget.position = currentEnemy.transform.position + finalDecision;
        currentTargetPosition = currentEnemy.transform.position + finalDecision;
        OutOfBoundsYCap();
        Debug.Log("NEW TARGET ACQUIRED FROM MISSING TARGET" + currentTargetPosition);
        Debug.Log("world angle against target" + Vector2.Angle(-Vector2.up,currentTargetPosition - currentEnemy.transform.position));
        


    }

    public void ResetForReuse(EnemyController enemyReplacement){
        currentEnemy = enemyReplacement;
        haveTarget = false;
        speed = enemyReplacement.GetSpeed();
        currentTargetPosition = currentEnemy.transform.position;
        //FindNewTarget(currentPlayer.position - currentEnemy.transform.position);

    }
    public void SetResourcesBasedOnType(EnemyMovementResources res){

        rotateTowardsInput = res.firstOption;
        currentPlayer = res.liveTransformTarget;
        
    }
    //public void SetCurrentTarget(Transform setTarget){
    //    currentTarget = setTarget;
    //}


    public void SetCurrentPlayerReference(Transform setTarget){
        currentPlayer = setTarget;
    }
    //enum for "random" next target finding strategy? idk lol
    public Vector2 getInputFromAngle(float ang){
        int targetdir =  Mathf.FloorToInt(ang/22.5f);
        //Debug.Log("direction!: "+ targetdir);
        Vector2 target = new Vector2();

        if(targetdir<0){
            switch(targetdir){
                case -8:
                    target.x = -1f;
                    target.y = 0f;
                    break;
                case -7:
                case -6:
                    target.x = -1f;
                    target.y = -1f;
                    break;
                case -5:
                case -4:
                    target.x = 0f;
                    target.y = -1f;
                    break;
                case -3:
                case -2:
                    target.x = 1f;
                    target.y = -1f;
                    break;
                case -1:
                    target.x = 1f;
                    target.y = 0f;
                    break;
            }
        }else{
            switch(targetdir){
                case 7:
                    target.x = -1f;
                    target.y = 0f;
                    break;
                case 6:
                case 5:
                    target.x = -1f;
                    target.y = 1f;
                    break;
                case 4:
                case 3:
                    target.x = 0f;
                    target.y = 1f;
                    break;
                case 2:
                case 1:
                    target.x = 1f;
                    target.y = 1f;
                    break;
                case 0:
                    target.x = 1f;
                    target.y = 0f;
                    break;
            }
        }
        Debug.Log("NOROTATETARGETINPUT " + target);
        return target.normalized;
    }
}
