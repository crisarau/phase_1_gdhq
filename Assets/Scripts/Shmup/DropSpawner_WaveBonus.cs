using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner_WaveBonus : MonoBehaviour
{

    private DropTableReader reader;
    [SerializeField]
    private UpgradeController ups;

    // Start is called before the first frame update
    void Start()
    {
        reader = transform.GetComponent<DropTableReader>();
    }

    public void GetBonus(){
        var tempDrop = reader.GetItem(false);
        var shouldPass = Instantiate(tempDrop.prefab, Vector3.zero, Quaternion.identity);
        if(shouldPass.name.Contains("Upgrade_Rolling")){
            //DO THE UPGRADEROLLINGLOGIC
            shouldPass.GetComponent<Upgrade_Rolling_Box>().SetValue(Random.Range(0,5));
        }else if(shouldPass.name.Contains("Null")){
            Debug.Log("WAVE BONUS WILL TRY FOR A LUCKY CHOICE");
            Destroy(shouldPass); //or should i just let it die naturally? idk
            LuckBasedSpawn();  
            return;
        }

        //place it in the field.
        shouldPass.transform.position = new Vector3(Random.Range(-3f,3f),6f,0);
        shouldPass.SetActive(true);
    }

    private void LuckBasedSpawn(){
        var deck = ups.ShareTypesCurrentlyInDeck();
        GameObject toSend;
        Drop upgradeDrop;
        if(deck==null){
            //just doing a normal upgrade, any of them
            upgradeDrop = reader.GetItem(true);
        }else{
            //SHOULD BE IN SAME ORDER IN ALTTABLE
            List<int> options = new List<int>();
            foreach (var item in deck)
            {
                if(item > -1){
                    options.Add(item);
                }

            }
            if(options.Count == 0){
                //if somehow all in deck maxed out...just get a random lol
                upgradeDrop = reader.GetItem(true);
            }else{
                //else give out the help needed
                upgradeDrop = reader.GetItem(options[Random.Range(0,options.Count)], true);
            }

            
        }

        //place it in the field.
        toSend = Instantiate(upgradeDrop.prefab, Vector3.zero, Quaternion.identity);
        toSend.transform.position = new Vector3(Random.Range(-3f,3f),6f,0);
        toSend.SetActive(true);
    }
}
