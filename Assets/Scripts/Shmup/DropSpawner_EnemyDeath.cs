using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner_EnemyDeath : MonoBehaviour
{
    private static DropTableReader reader;

    // Start is called before the first frame update
    void Start()
    {
        reader = transform.GetComponent<DropTableReader>();
    }

    public static void GetBonus(bool altFlag, Vector3 whereToSpawn){
        //this one depends on data from the enemy, also made it a thing.
        var tempDrop = reader.GetItem(altFlag);
        
        var shouldPass = Instantiate(tempDrop.prefab, whereToSpawn, Quaternion.identity);
        if(shouldPass.name.Contains("Null")){
            Debug.Log("ENEMY Spawner will Spawn Nothing");
            Destroy(shouldPass);
            return;
        }
        shouldPass.SetActive(true);
    }

    public static void AlterLiveNothingValues(bool altFlag,int stage, bool increase){
        if(increase){
            reader.IncreaseNothingValue(stage,altFlag);
        }else{
            reader.DecreaseNothingValue(stage,altFlag);
        }
    }
    public static void ResetAlteredTables(){
        reader.ResetTableWeights(true);
        reader.ResetTableWeights(false);
    }

}
