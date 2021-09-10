using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyLiveData{
    public bool isInUse;
    //public int parentWave;
    //public EnemyController enemyController;
    public GameObject enemyGameObjectInstance;
    public int placeIndex;

    //for quick find in ability and movement manager.
    public int abilityOption;
    public int movementOption;
    public int maneuverOption;
}

public class EnemyMovementResources{
    public GameObject pathObject;
    public Vector3 pathPlacement;
    public List<EnemyMovementInputs> inputsList;
    public bool firstOption;

    public Transform liveTransformTarget;
}

public class EnemyAbilityResources{
    public Transform liveTransformTarget;
    public bool firstToggleOption;
}

public class EnemyManager
{
    WaveEntitySO currentWaveObjectToSpawn;
    GameObject baseEnemyTemplate;
    Transform baseEnemyParent;//meant to be innactive in scene
    Transform playerReferencePosition;
    public int tableSize;
    public static List<EnemyLiveData> enemyTable;

    private static EnemyMovementManager enemyMovementManager;
    private static EnemyAbilityManager enemyAbilityManager;

    public EnemyManager(int size, Transform parent, Transform playerRef, GameObject enemyTemplatePrefab){
        
        enemyTable = new List<EnemyLiveData>();
        tableSize = size;
        baseEnemyParent = parent;
        enemyMovementManager = new EnemyMovementManager(15);
        enemyAbilityManager = new EnemyAbilityManager(5);
        playerReferencePosition = playerRef;
        baseEnemyTemplate = enemyTemplatePrefab;
    }


    public void InitializeTable(){
        for(int i = 0;i<tableSize; i++){
            EnemyLiveData eldtemp = new EnemyLiveData();
            eldtemp.isInUse = false;
            //eldtemp.parentWave = -1;
            eldtemp.placeIndex = i;
            eldtemp.enemyGameObjectInstance = Object.Instantiate(baseEnemyTemplate, baseEnemyParent.transform);
            enemyTable.Add(eldtemp);
        }

        enemyMovementManager.InitializeTables();
        enemyAbilityManager.InitializeTables();
    }

    public void Spawn(WaveEntitySO data, bool isFromRandomBank, int gameplayWaveBelongingTo){

        //is there an enemy abailable? or do we need to expand?
        int tempEnemyIndex = CheckForEnemyTemplateAvailable();

        EnemyController tempEC = EnemyManager.enemyTable[tempEnemyIndex].enemyGameObjectInstance.GetComponent<EnemyController>();
        tempEC.SetPlaceInManager(tempEnemyIndex);
        tempEC.SetGameplayWaveBelongingTo(gameplayWaveBelongingTo);

        //what is everything we need to setup?
        //what kind of  enemy is it?...EnemyStatSO.
        
        tempEC.SetAbilityBoundaries(data.minAbilityLaunchTimeCycle, data.maxAbilityLaunchTimeCycle);
        tempEC.SetManeuverBoundaries(data.colliderTimeAlive, data.minEvasiveColliderActivateTime);
        tempEC.SetStats(SetEnemyType(data.EnemyType));

        tempEC.SetDropTableOption(data.dropTableOption);

        //what kind of projectile will it launch?...WeaponSO
        tempEC.SetWeapon(SetWeaponProjectileType(data.WeaponType));

        //Shoot behavior. bullets at once int, min and max angle float, random spread
        //conscutive current shot frequency min and max floats
        tempEC.SetShootingBehaviors(data.ShootBehavior, data.consecutiveShotsBank, data.MinShotFrequency, data.MaxShotFrequency);

        //ShootControllerSettings
        tempEC.SetShootControllerSettings(data.bulletsAtOnce, data.minAngle,data.maxAngle,data.RandomSpreadShot);
        
        EnemyMovementResources tempMoveResources = new EnemyMovementResources();
        tempMoveResources.pathObject = data.path;
        tempMoveResources.pathPlacement = data.pathPlacement;
        tempMoveResources.inputsList = data.inputs;
        tempMoveResources.firstOption = data.rotationTowardsTarget;

        //about dealing with transforms...how to do this? like we could just do it here and ask for the player's transform or from a random thing on screen like a power up.
        //by default just send your player lol
        tempMoveResources.liveTransformTarget = playerReferencePosition;

        //Moverment Type... will it follow a sequence? how about a path or random?
        tempEC.SetMovementType(SetEnemyMovementType(data.MovementOptionType,tempEnemyIndex, tempMoveResources)); 
        EnemyManager.enemyTable[tempEnemyIndex].movementOption = data.MovementOptionType;
        
        EnemyAbilityResources tempAbilityResources = new EnemyAbilityResources();
        tempAbilityResources.liveTransformTarget = playerReferencePosition;

        //Ability Type, negative is no ability
        if(data.AbilityType >=0){
            tempEC.SetAbilityType(SetAbilityType(data.AbilityType,tempEnemyIndex,tempAbilityResources));
            EnemyManager.enemyTable[tempEnemyIndex].abilityOption = data.AbilityType;
        }
        
        //Reactive Ability Type
        if(data.EvasiveManeuverType >= 0){
            tempEC.SetManeuverType(SetManeuverType(data.EvasiveManeuverType,tempEnemyIndex,tempAbilityResources));
            EnemyManager.enemyTable[tempEnemyIndex].maneuverOption = data.EvasiveManeuverType;
        }
        
        //powerup??? shields?

        //exit strategy? timer till retreat
        tempEC.SetEnemyExitStrategy(data.exitType);

        if(data.autoAim){
            tempEC.ActivateAutomaticAim();
        }
        
        

        
        
        
        //finally set a delay for the next spawn if allowed.
        if(isFromRandomBank){
            LevelController.AddToRandomDelay(data.delay);
        }else{
            LevelController.AddToOrderedDelay(data.delay);
        }
        
        // activate at xyz position... If we in field, make an in place teleport vfx
        //Initiate Enemy!
        //set enemy lifetime
        
        tempEC.SetLifetime(Time.time + data.lifeTime);
        tempEC.transform.position = data.creationPosition;
        tempEC.transform.gameObject.SetActive(true);
        Debug.Log("new spawn with this data:" + enemyTable[tempEnemyIndex].movementOption);
    }

    private int CheckForEnemyTemplateAvailable(){
        for(int j = 0; j <  enemyTable.Count; j++){
            if(enemyTable[j].isInUse==false){
                //we found one!
                enemyTable[j].isInUse = true;

                return j;
            }
        }
        //SOMEHOW WE RAN OUT OF SPACE TO SPAWN MORE!
        return -1;
    }

    private Weapon SetWeaponProjectileType(int projectileType){
        Weapon projectileChosen;
        switch (projectileType)
        {
        //case 5:
        //    
        //    break;
        //case 4:
        //    
        //    break;
        case 3:
            projectileChosen = Resources.Load<Weapon>("Enemy_HomingMissile");            
            break;
        case 2:
            projectileChosen = Resources.Load<Weapon>("Enemy_Pea");
            break;
        case 1:
            projectileChosen = Resources.Load<Weapon>("Enemy_Single");
            break;
        default:
            projectileChosen = Resources.Load<Weapon>("Enemy_Single");
            break;
        }
        return projectileChosen;
    }

    private EnemyStats SetEnemyType(int enemyType){
        EnemyStats enemyTypeChosen;
        switch (enemyType)
        {
        case 4:
            enemyTypeChosen = Resources.Load<EnemyStats>("Tanker");    
            break;
        case 3:
            enemyTypeChosen = Resources.Load<EnemyStats>("Flyer");    
            break;
        case 2:
            enemyTypeChosen = Resources.Load<EnemyStats>("Zipper");
            break;
        case 1:
            enemyTypeChosen = Resources.Load<EnemyStats>("Trooper");
            break;
        default:
            enemyTypeChosen = Resources.Load<EnemyStats>("Trooper");
            break;
        }
        return enemyTypeChosen;
    }

    private IMovementOption SetEnemyMovementType(int enemyType, int enemyIndex, EnemyMovementResources res){
        //if we know that we are doing enemy movement type...shouldn't we take into account our loadout? Resources needed to act...what resources are those?
        //if we have an input list...pass it down accordingly, if we have a path to follow, pass that down accordingly, if we have a bool option, pass that down too.
        return enemyMovementManager.GetMovementOption(enemyType, enemyIndex, res);
    }

    private IEnemyAbility SetAbilityType(int abilityType, int enemyIndex, EnemyAbilityResources res){
        return enemyAbilityManager.GetAbilityOption(abilityType, enemyIndex, res);
    }

    private IEnemyEvasionManeuver SetManeuverType(int abilityType, int enemyIndex, EnemyAbilityResources res){
        return enemyAbilityManager.GetManeuverOption(abilityType, enemyIndex, res);
    }

    public static void ReturnEnemyToPool(int index){

        if(EnemyManager.enemyTable[index].enemyGameObjectInstance.active == false){
            //no need to do this if innactive, i did this in case of double hit at same time...which was happening with homing attacks
            Debug.Log("homing same hit case");
            return;
        }
        //if (EnemyManager.enemyTable[index].movementOption != -1)
        //put back its abilities into the bank as well!
        enemyMovementManager.ReturnToPool(index, EnemyManager.enemyTable[index]);
        enemyAbilityManager.ReturnToPool(index, EnemyManager.enemyTable[index]);

        EnemyManager.enemyTable[index].isInUse = false;
        //EnemyManager.enemyTable[index].parentWave = -1;
        EnemyManager.enemyTable[index].abilityOption = -1;
        EnemyManager.enemyTable[index].maneuverOption = -1;
        EnemyManager.enemyTable[index].movementOption = -1;

        //you need to set as innactive! make the enemies do that themselves!!!
        EnemyManager.enemyTable[index].enemyGameObjectInstance.SetActive(false);
    }

}
//NOTE SOMETHING I DON'T GET AND SHOULD EXPERIMENT WITH...
//should i just cache the Scriptable Object branches once and share it instead of going into Resources to load? That would be slow lol.
//let's TEST IT OUT!