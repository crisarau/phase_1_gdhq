using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyMovementInputs{
    public float x;
    public float y;
    public int times;

    public EnemyMovementInputs(float hor,float vert, int repeats){
        x = hor;
        y = vert;
        times = repeats;
    }
}
[System.Serializable]
public class EM_InputSequence : IMovementOption
{

    [SerializeField]
    private List<EnemyMovementInputs> movementInputs;
    private int movementIndex;
    private int timesInCurrentInput;
    private bool loopSequence; //do we loop after ending?
    private float speed;

    private float horizontalInput;
    private float verticalInput;

    private Enemy currentEnemy;

    public EM_InputSequence(Enemy enemy,bool loop, List<EnemyMovementInputs> inputs, float initialSpeed){
        currentEnemy = enemy;
        movementIndex = 0;
        timesInCurrentInput = 0;
        loopSequence = loop;
        movementInputs = inputs;
        speed = initialSpeed;
    }
    public void Move(){
        //go through input sequence and apply it to character
        if(movementIndex <  movementInputs.Count){
            if(timesInCurrentInput < movementInputs[movementIndex].times){
                SingleMove();
                timesInCurrentInput += 1;

            }else{
                timesInCurrentInput = 0;
                movementIndex += 1;
                if(movementIndex < movementInputs.Count){
                    SingleMove();
                }else{
                    if(CheckForLoopingSequence()){
                        SingleMove();
                    }        
                }
                
            }

        }else{
            if(CheckForLoopingSequence()){
                SingleMove();
            }
            //do nothing, end of sequence lol
        }
    }

    private bool CheckForLoopingSequence(){
        if(loopSequence){
            movementIndex = 0;
        }
        return loopSequence;
    }
    private void SingleMove(){
        horizontalInput = movementInputs[movementIndex].x;
        verticalInput = movementInputs[movementIndex].y;
        Vector3 direction = new Vector3(horizontalInput,verticalInput,0).normalized;

        currentEnemy.transform.Translate(direction *  speed * Time.deltaTime);
                
    }

    public void SetInputSequence(List<EnemyMovementInputs> inputs){
        movementInputs = inputs;
    }
    public void SetRepeat(bool set){
        loopSequence = set;
    }
}
