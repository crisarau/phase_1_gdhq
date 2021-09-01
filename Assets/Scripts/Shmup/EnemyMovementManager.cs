using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovementLiveData{
    public bool isInUse;
    public int enemyOwnerIndex;
    public IMovementOption enemyGameObjectInstance;
}
public class EnemyMovementManager
{
    int tablesizes;

    Dictionary<int, List<EnemyMovementLiveData>> movementTables = new Dictionary<int, List<EnemyMovementLiveData>>();

    

    public EnemyMovementManager(int storage){
        tablesizes = storage;
        movementTables.Add(0,new List<EnemyMovementLiveData>());
        movementTables.Add(1,new List<EnemyMovementLiveData>());
        movementTables.Add(2,new List<EnemyMovementLiveData>());
        movementTables.Add(3,new List<EnemyMovementLiveData>());
        //for now only two option types...inputs and go to o
        //movementTables[0].Add(new EM_GoToPosition(null, null, true, 5f)); this WORKS?!!

    }

    
    public void InitializeTables(){
        int tempTableIndex = 0;
        foreach(var table in movementTables.Values){
            for(int i = 0;i<tablesizes; i++){
                //table.Add(new EM_GoToPosition(null, null, false, 0f));
                table.Add(InitializeTableBranches(tempTableIndex));
            }
            tempTableIndex += 1;    
        }
    }

    private EnemyMovementLiveData InitializeTableBranches(int option){
        EnemyMovementLiveData temp = new EnemyMovementLiveData();
        temp.isInUse = false;
        temp.enemyOwnerIndex = -1;
                
        switch (option)
        {
            case 0:
                temp.enemyGameObjectInstance = new EM_InputSequence(null, true, null, 0f);
                break;
            case 1:
                temp.enemyGameObjectInstance = new EM_PathFollow(null, null, 0f, false);
                break;
            case 2:
                //THIS SHOULD ALWAYS POINT TO PLAYER...or power up? idk lol...WE ARE NOT USING THIS ANYMORE
                temp.enemyGameObjectInstance =  new EM_GoToPosition(null, null, null, false, 0f);
                break;
            case 3:
                //THIS SHOULD ALWAYS POINT TO PLAYER...or power up? idk lol
                temp.enemyGameObjectInstance =  new EM_RandomPosition(null, null, null, false, 0f);
                break;
            default:
                temp.enemyGameObjectInstance = new EM_InputSequence(null, true, null, 0f);
                break;
        }
        return temp;
    }

    public IMovementOption GetMovementOption(int option, int index, EnemyMovementResources res){
        //search for the appropriate dictionary...
        //you should error check for non existant option just in case...
        
        //check if any available...and then return the reference
        foreach(var instance in movementTables[option]){
            if(instance.isInUse == false){
                instance.isInUse = true;
                instance.enemyOwnerIndex = index;

                //RESET ACCORDINGLY
                instance.enemyGameObjectInstance.SetResourcesBasedOnType(res);
                instance.enemyGameObjectInstance.ResetForReuse(EnemyManager.enemyTable[index].enemyGameObjectInstance.GetComponent<EnemyController>());
                Debug.Log("EMM: GIVING ENEMY: " + index + "MOVEMENT OF TYPE: " + option);                

                return instance.enemyGameObjectInstance;
            }
        }
        //NONE AVAILABLE!!! WHAT TO DO?!
        return null;
    }

    public void ReturnToPool(int owner, EnemyLiveData data){
        //if(data.movementOption < 0){
        //    Debug.Log("movement type " + data.movementOption + " from " + owner + " returning to pool");
            //why would we get a -1 in the first place???
            
        //    movementTables[0][owner].isInUse = false;
        //    return;
        //}
        
        //this doesnt make sense...instead we should be looking for the one that has the same enemyOwnerIndex...also reset it to -1
        //movementTables[data.movementOption][owner].isInUse = false;
        foreach(var liveData in movementTables[data.movementOption]){
            if(liveData.enemyOwnerIndex == owner){
                Debug.Log("EMM: RETURNING FROM ENEMY: " + owner + "MOVEMENT OF TYPE: " + data.movementOption);                

                //we found it...reset!
                liveData.isInUse = false;
                liveData.enemyOwnerIndex = -1;
            }
        }
    }
}
