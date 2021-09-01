using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAbilityLiveData{
    public bool isInUse;
    public int enemyOwnerIndex;
    public IEnemyAbility enemyGameObjectInstance;
}
public class EnemyAbilityManager
{
    //im thinking that there is no need to do the whole separation of what is an ability and what is a reactive maneuver...but idk...

    int tablesizes;

    Dictionary<int, List<EnemyAbilityLiveData>> abilityTables = new Dictionary<int, List<EnemyAbilityLiveData>>();
    Dictionary<int, List<EnemyAbilityLiveData>> maneuverTables = new Dictionary<int, List<EnemyAbilityLiveData>>();

        

    public EnemyAbilityManager(int storage){

        tablesizes = storage;
        //ram, aim shot, sneaky, aim spin
        abilityTables.Add(0,new List<EnemyAbilityLiveData>());
        abilityTables.Add(1,new List<EnemyAbilityLiveData>());
        abilityTables.Add(2,new List<EnemyAbilityLiveData>());
        abilityTables.Add(3,new List<EnemyAbilityLiveData>());
        //dodge
        maneuverTables.Add(0,new List<EnemyAbilityLiveData>());

    }

    public void InitializeTables(){
        int tempTableIndex = 0;
        foreach(var table in abilityTables.Values){
            for(int i = 0;i<tablesizes; i++){
                //table.Add(new EM_GoToPosition(null, null, false, 0f));
                table.Add(InitializeTableAbilityBranches(tempTableIndex));
            }
            tempTableIndex += 1;
        }

        tempTableIndex = 0;
        foreach(var table in maneuverTables.Values){
            for(int i = 0;i<tablesizes; i++){
                //table.Add(new EM_GoToPosition(null, null, false, 0f));
                table.Add(InitializeTableManeuverBranches(tempTableIndex));
            }
            tempTableIndex += 1;
        }
    }

    private EnemyAbilityLiveData InitializeTableAbilityBranches(int option){
        EnemyAbilityLiveData temp = new EnemyAbilityLiveData();
        temp.isInUse = false;
        temp.enemyOwnerIndex = -1;
                
        switch (option)
        {
            case 0:
                temp.enemyGameObjectInstance = new EA_Ram(null, 0.5f, 0f, 0f,0f, null);
                break;
            //case 1:
            //    temp.enemyGameObjectInstance =  new EM_GoToPosition(null, null, false, 0f);
            //    break;
            default:
                temp.enemyGameObjectInstance = new EA_Ram(null, 0.5f, 0f, 0f,0f, null);
                break;
        }
        return temp;
    }

    private EnemyAbilityLiveData InitializeTableManeuverBranches(int option){
        EnemyAbilityLiveData temp = new EnemyAbilityLiveData();
        temp.isInUse = false;
        temp.enemyOwnerIndex = -1;
                
        switch (option)
        {
            case 0:
                temp.enemyGameObjectInstance = new EA_Dodge(null,0f,0f);
                break;
            default:
                temp.enemyGameObjectInstance = new EA_Dodge(null,0f,0f);
                break;
        }
        return temp;
    }

    public IEnemyAbility GetAbilityOption(int option, int index, EnemyAbilityResources res){
        //search for the appropriate dictionary...
        //you should error check for non existant option just in case...
        
        //check if any available...and then return the reference
        foreach(var instance in abilityTables[option]){
            if(instance.isInUse == false){
                instance.isInUse = true;
                instance.enemyOwnerIndex = index;
                Debug.Log("EAM: GIVING ENEMY: " + index + " ABILITY OF TYPE: " + option);                
                
                instance.enemyGameObjectInstance.SetResourcesBasedOnType(res);
                instance.enemyGameObjectInstance.ResetForReuse(EnemyManager.enemyTable[index].enemyGameObjectInstance.GetComponent<EnemyController>());

                return instance.enemyGameObjectInstance;
            }
        }
        //NONE AVAILABLE!!! WHAT TO DO?!
        return null;
    }

    public IEnemyEvasionManeuver GetManeuverOption(int option, int index, EnemyAbilityResources res){
        //search for the appropriate dictionary...
        //you should error check for non existant option just in case...
        
        //check if any available...and then return the reference
        foreach(var instance in maneuverTables[option]){
            if(instance.isInUse == false){
                instance.isInUse = true;
                instance.enemyOwnerIndex = index;
                Debug.Log("EAM: GIVING ENEMY: " + index + " EVASIVE ABILITY OF TYPE: " + option);                

                instance.enemyGameObjectInstance.SetResourcesBasedOnType(res);
                instance.enemyGameObjectInstance.ResetForReuse(EnemyManager.enemyTable[index].enemyGameObjectInstance.GetComponent<EnemyController>());

                return instance.enemyGameObjectInstance as IEnemyEvasionManeuver;
            }
        }
        //NONE AVAILABLE!!! WHAT TO DO?!
        return null;
    }

    public void ReturnToPool(int owner, EnemyLiveData data){
        //find the owner
        //abilityTables[owner].is

        Debug.Log("OWNER IS: " + owner);

        //return both maneuver and this IF IT HAD ONE...so keep at -1 i guess if done.
        if(data.abilityOption >= 0){
            //abilityTables[data.abilityOption][owner].isInUse = false;

            foreach(var liveData in abilityTables[data.abilityOption]){
                if(liveData.enemyOwnerIndex == owner){
                    //we found it...reset!
                    liveData.isInUse = false;
                    liveData.enemyOwnerIndex = -1;
                    Debug.Log("EAM: RETURNING FROM ENEMY: " + owner + "ABILITY OF TYPE: " + data.abilityOption);                

                }
            }
        }
        if(data.maneuverOption >=0){
            //maneuverTables[data.maneuverOption][owner].isInUse = false;
            foreach(var liveData in maneuverTables[data.maneuverOption]){
                if(liveData.enemyOwnerIndex == owner){
                    //we found it...reset!
                    liveData.isInUse = false;
                    liveData.enemyOwnerIndex = -1;
                    Debug.Log("EAM: RETURNING FROM ENEMY: " + owner + "EVASIVE ABILITY OF TYPE: " + data.maneuverOption);                

                }
            }
        }
    }

}
