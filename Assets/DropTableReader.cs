using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DropTableReader : MonoBehaviour
{
    [SerializeField]
    private DropTableSO tableSO; //the table we get our initial weights from!!!
    [SerializeField]
    private DropTableSO altTableSO; //each component will use this altTable differently.

    
    [SerializeField]
    private List<Drop> table;
    [SerializeField]
    private List<Drop> altTable;
    private bool isInitialized;
    private float totalWeight;

    private bool beenAltered;


    //RULES
    //ALL HAVE NOTHING as their first value.
    //TO DECREASE OR INCREASE BY AN AMMOUNT... IF 5 in table and one goes down  increase others by (amount increased / 4)


    private void Start() {
        InitializeLiveTable();
        if(altTableSO != null){
            InitializeLiveAltTable();
        }
    }

    private void InitializeLiveTable(){

        for (int i = 0; i < tableSO.drops.Count; i++)
        {
            table.Add(tableSO.GetDrop(i));
        }
        //for some reason this would give me a  deep copy...which i DONT want
        //foreach (var item in tableSO.drops)
        //{   
        //    table.Add(item);
        //}
    }
    private void InitializeLiveAltTable(){
        for (int i = 0; i < altTableSO.drops.Count; i++)
        {
            altTable.Add(altTableSO.GetDrop(i));
        }
        //foreach (var item in altTableSO.drops)
        //{   
        //    altTable.Add(item);
        //}
    }

    private void InitializeWeightTotal(){
	    if(!isInitialized){
		    totalWeight = table.Sum(item => item.weight);
		    isInitialized = true;
        }
    }

    public Drop GetItem(bool altFlag){
        if(!altFlag){
            return GetRandomItem();
        }else{
            return GetRandomItemFromAlternativeTable();
        }

    }

    public Drop GetItem(int index, bool altFlag){
        if(!altFlag){
            return table[index];
        }else{
            return altTable[index];
        }

    }
    private Drop GetRandomItem(){
	    InitializeWeightTotal();
	    float diceRoll = Random.Range(0f, totalWeight);
	    foreach (var item in table){
		    if (item.weight >= diceRoll){
			    return item;
            }
            diceRoll -= item.weight;
        }
        Debug.Log("Return NOTHING");
        return null;
	    //throw new system.exception "reward generation failed"
    }
    private Drop GetRandomItemFromAlternativeTable(){
	    InitializeWeightTotal(); //why even do this?
	    float diceRoll = Random.Range(0f, totalWeight);
	    foreach (var item in altTable){
		    if (item.weight >= diceRoll){
			    return item;
            }
            diceRoll -= item.weight;
        }
        Debug.Log("Return NOTHING");
        return null;
	    //throw new system.exception "reward generation failed"
    }

    public void DecreaseNothingValue(int value, bool altFlag){
        //Debug.Log("DECREASING NOTHING, INCREASING OTHERS");
        //i guess i will have to make the first one the nothing always? YES lol
        if(!altFlag){

                table[0].weight -= 5f;
                float increaseBy = 5f/(table.Count-1f);
                for (int i = 1; i < table.Count; i++){
                    table[i].weight += increaseBy;
                }
            
        }else{
                altTable[0].weight -= 5f;
                float increaseBy = 5f/(altTable.Count-1f);
                for (int i = 1; i < altTable.Count; i++){
                    altTable[i].weight += increaseBy;
                }
        }


        
        
    }
    public void IncreaseNothingValue(int value, bool altFlag){
        //i guess i will have to make the first one the nothing?
        //or do i have to look for it?
        //Debug.Log("INCREASING NOTHING, DECREASING OTHERS");
        if(!altFlag){
                table[0].weight += 5f;
                float decreaseBy = 5f/(table.Count-1f);
                for (int i = 1; i < table.Count; i++){
                    table[i].weight -= decreaseBy;
                }
            
        }else{
                altTable[0].weight += 5f;
                float decreaseBy = 5f/(altTable.Count-1f);
                for (int i = 1; i < altTable.Count; i++){
                    altTable[i].weight -= decreaseBy;
                }
            
        }
    }
    
    //is this creating a deep copy too? i think it shouldn't since we only getting numbers tho :c i'd need to do a replacement AND then try to increase
    public void ResetTableWeights(bool altFlag){
        if(!altFlag){
            for (int i = 0; i < tableSO.drops.Count; i++)
            {
                table[i].weight = tableSO.drops[i].weight;
            }
        }else{
            for (int i = 0; i < altTableSO.drops.Count; i++)
            {
                altTable[i].weight = altTableSO.drops[i].weight;
            }
        }

    }

}
