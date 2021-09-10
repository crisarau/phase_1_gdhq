using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner_Timed : MonoBehaviour
{

    private static DropTableReader reader;

    public float timeToSpawn;

    [SerializeField]
    public float minSpawn;
    [SerializeField]
    public float maxSpawn;


    void Start()
    {
        reader = transform.GetComponent<DropTableReader>();
        setTimeToSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= timeToSpawn){
            GetBonus();
            setTimeToSpawn();
        }
    }

    void setTimeToSpawn(){
        timeToSpawn = Time.time + Random.Range(minSpawn,maxSpawn);
    }

    public static void GetBonus(){
        var tempDrop = reader.GetItem(false);
        var shouldPass = Instantiate(tempDrop.prefab, Vector3.zero, Quaternion.identity);
        if(shouldPass.name.Contains("BasicTimerUpgrade")){
            //we picked an upgrade
            var upgradeDrop = reader.GetItem(true);
            shouldPass = Instantiate(upgradeDrop.prefab, Vector3.zero, Quaternion.identity);
            
        }else if(shouldPass.name.Contains("Null")){
            Debug.Log("Timed Spawner will Spawn Nothing");
            Destroy(shouldPass);
            return;
        }

        //place it in the field.
        shouldPass.transform.position = new Vector3(Random.Range(-3f,3f),6f,0);
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
