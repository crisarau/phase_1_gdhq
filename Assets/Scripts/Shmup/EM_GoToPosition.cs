using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM_GoToPosition : IMovementOption
{
    private Transform currentTarget;
    private bool setRandomTarget;
    private bool lockX;
    private bool lockY;

    private float destinationRange;

    private Enemy currentEnemy;

    private float speed; // speed obtained from stats of character

    public EM_GoToPosition(Enemy enemy, Transform target, bool setRandom,float initialSpeed){
        currentTarget = target;
        setRandomTarget = setRandom;
        currentEnemy = enemy;
        speed = initialSpeed;
    }
    public void Move()
    {
        Vector3 dir = currentTarget.position - currentEnemy.transform.position;
        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        Debug.Log("angle: "+ angle);


        //keep moving towards the current transform target
        currentEnemy.transform.Translate(getInputFromAngle(angle) *  speed * Time.deltaTime);
    }

    public void SetCurrentTarget(Transform setTarget){
        currentTarget = setTarget;
    }
    //enum for "random" next target finding strategy? idk lol
    public Vector2 getInputFromAngle(float ang){
        int targetdir =  Mathf.FloorToInt(ang/22.5f);
        Debug.Log("direction!: "+ targetdir);
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

        return target.normalized;
    }
}
