using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IUpgradePickUps{
    void AddPassiveEffect(Transform player);
    void AddActiveEffect(Transform player);
    void RemovePassiveEffect(Transform player);
    void RemoveActiveEffect(Transform player);

}

public class BaseUpgrade:IUpgradePickUps{

    public int currentStage;
    public int type;
    public int possibleStages;
   

    public virtual void AddPassiveEffect(Transform player){}
    public virtual void AddActiveEffect(Transform player){}
    public virtual void RemovePassiveEffect(Transform player){}
    public virtual void RemoveActiveEffect(Transform player){}

    public virtual void Upgrade(Transform player){}
    public virtual void Downgrade(Transform player){}
    public virtual void ResetAllFromUpgrade(Transform player){}
}

public class MultiShotUpgrade : BaseUpgrade{

    //a set of children... (1,2) in front (3,4) on angles, 5,6 on sides
    //for now we are only doing it sequencially in this order. 

    public MultiShotUpgrade(){
        type = 0;
        currentStage = 0;
        possibleStages = 2;//meaning there are 3 stages
    }

    public override void AddActiveEffect(Transform player)
    {
        Debug.Log("UPGRADE OF TYPE " + type + " ON STAGE "+ currentStage + " ACTIVE");
    }

    public override void AddPassiveEffect(Transform player)
    {
        player.GetComponent<Player>()._shotSpawns.GetChild(1).gameObject.SetActive(true);
        player.GetComponent<Player>()._shotSpawns.GetChild(2).gameObject.SetActive(true);
    }
    public override void RemovePassiveEffect(Transform player){
        player.GetComponent<Player>()._shotSpawns.GetChild(1).gameObject.SetActive(false);
        player.GetComponent<Player>()._shotSpawns.GetChild(2).gameObject.SetActive(false);
    }
    public override void RemoveActiveEffect(Transform player){
        Debug.Log("UPGRADE OF TYPE " + type + " IN-ACTIVE");
    }

    public override void Upgrade(Transform player){
        Debug.Log("UPGRADING MULTISHOT");
        if(currentStage == possibleStages){
            Debug.Log("all maxed out!");
        }
        if(currentStage == 0){
            player.GetComponent<Player>()._shotSpawns.GetChild(3).gameObject.SetActive(true);
            player.GetComponent<Player>()._shotSpawns.GetChild(4).gameObject.SetActive(true);
        }else if(currentStage == 1){
            player.GetComponent<Player>()._shotSpawns.GetChild(5).gameObject.SetActive(true);
            player.GetComponent<Player>()._shotSpawns.GetChild(6).gameObject.SetActive(true);
        }
        currentStage +=1;
    }

    public override void Downgrade(Transform player){
        //if we already at 0 this doesn't get called. it straight up goes to be removed

        if(currentStage == 2){
            player.GetComponent<Player>()._shotSpawns.GetChild(5).gameObject.SetActive(false);
            player.GetComponent<Player>()._shotSpawns.GetChild(6).gameObject.SetActive(false);
        }else if(currentStage == 1){
            player.GetComponent<Player>()._shotSpawns.GetChild(3).gameObject.SetActive(false);
            player.GetComponent<Player>()._shotSpawns.GetChild(4).gameObject.SetActive(false);
        }
        currentStage -=1;
    }
    public override void ResetAllFromUpgrade(Transform player)
    {
        player.GetComponent<Player>().DeactivateAllMultiShots();
    }

}

public class MirrorShotUpgrade : BaseUpgrade{

    
    public MirrorShotUpgrade(){
        type = 1;
        currentStage = 0;
        possibleStages = 1;//meaning there are 2 stages
    }

    public override void AddActiveEffect(Transform player)
    {
        Debug.Log("UPGRADE OF TYPE " + type + "ON STAGE "+ currentStage + " ACTIVE");
    }

    public override void AddPassiveEffect(Transform player)
    {
        player.GetComponent<Player>().ChangeCurrentMirrorShotLevel(0);
    }
    public override void RemovePassiveEffect(Transform player){
        player.GetComponent<Player>().ChangeCurrentMirrorShotLevel(-1);
    }
    public override void RemoveActiveEffect(Transform player){
        Debug.Log("UPGRADE OF TYPE " + type + " IN-ACTIVE");
    }

    public override void Upgrade(Transform player){
        
        currentStage +=1;
        player.GetComponent<Player>().ChangeCurrentMirrorShotLevel(currentStage);
    }

    public override void Downgrade(Transform player){
        //if we already at 0 this doesn't get called. it straight up goes to be removed

        currentStage -=1;
        player.GetComponent<Player>().ChangeCurrentMirrorShotLevel(currentStage);
    }

    public override void ResetAllFromUpgrade(Transform player)
    {
        player.GetComponent<Player>().ChangeCurrentMirrorShotLevel(-1);
        //do nothing...removepassive effect takes care of it
    }
}

public class HomingShotUpgrade : BaseUpgrade{

    
    public HomingShotUpgrade(){
        type = 2;
        currentStage = 0;
        possibleStages = 2;//meaning there are 3 stages
    }

    public override void AddActiveEffect(Transform player)
    {
        Debug.Log("UPGRADE OF TYPE " + type + "ON STAGE "+ currentStage + " ACTIVE");
        player.GetComponent<Player>().ChangeHomingChargeSpeed(true);
    }

    public override void AddPassiveEffect(Transform player)
    {
        player.GetComponent<Player>().ChangeHomingLevel(0);
    }
    public override void RemovePassiveEffect(Transform player){
        player.GetComponent<Player>().ChangeHomingLevel(-1);
    }
    public override void RemoveActiveEffect(Transform player){
        player.GetComponent<Player>().ChangeHomingChargeSpeed(false);
    }

    public override void Upgrade(Transform player){
        
        currentStage +=1;
        player.GetComponent<Player>().ChangeHomingLevel(currentStage);
    }

    public override void Downgrade(Transform player){
        //if we already at 0 this doesn't get called. it straight up goes to be removed

        currentStage -=1;
        player.GetComponent<Player>().ChangeHomingLevel(currentStage);
    }

    public override void ResetAllFromUpgrade(Transform player)
    {
        player.GetComponent<Player>().ChangeHomingLevel(-1);
    }

}

public class LuckyUpgrade : BaseUpgrade{

    
    public LuckyUpgrade(){
        type = 3;
        currentStage = 0;
        possibleStages = 2;//meaning there are 3 stages
    }

    public override void AddActiveEffect(Transform player)
    {
        Debug.Log("UPGRADE OF TYPE " + type + "ON STAGE "+ currentStage + " ACTIVE");
        player.GetComponent<Player>().ChangeMagnetStatus(true);
    }

    public override void AddPassiveEffect(Transform player)
    {
        Debug.Log("add code to activate luck increase plz");
        player.GetComponent<Player>().ChangeCurrentCriticalProbability(0);

        DropSpawner_Timed.AlterLiveNothingValues(false,currentStage,false);
        DropSpawner_EnemyDeath.AlterLiveNothingValues(false,currentStage,false);
        DropSpawner_EnemyDeath.AlterLiveNothingValues(true,currentStage,false);

    }
    public override void RemovePassiveEffect(Transform player){
        player.GetComponent<Player>().ChangeCurrentCriticalProbability(-1);

        DropSpawner_Timed.AlterLiveNothingValues(false,currentStage,true);
        //increase EnemyDrop's Table AND ALT
        DropSpawner_EnemyDeath.AlterLiveNothingValues(false,currentStage,true);
        DropSpawner_EnemyDeath.AlterLiveNothingValues(true,currentStage,true);
    }
    public override void RemoveActiveEffect(Transform player){
        Debug.Log("UPGRADE OF TYPE " + type + " IN-ACTIVE");
        player.GetComponent<Player>().ChangeMagnetStatus(false);
    }

    public override void Upgrade(Transform player){
        

        //increase BasicTimer's Table only
        DropSpawner_Timed.AlterLiveNothingValues(false,currentStage,false);
        //increase EnemyDrop's Table AND ALT
        DropSpawner_EnemyDeath.AlterLiveNothingValues(false,currentStage,false);
        DropSpawner_EnemyDeath.AlterLiveNothingValues(true,currentStage,false);

        currentStage +=1;
        player.GetComponent<Player>().ChangeCurrentCriticalProbability(currentStage);
    }

    public override void Downgrade(Transform player){
        //if we already at 0 this doesn't get called. it straight up goes to be removed
        

        //increase BasicTimer's Table only
        DropSpawner_Timed.AlterLiveNothingValues(false,currentStage,true);
        //increase EnemyDrop's Table AND ALT
        DropSpawner_EnemyDeath.AlterLiveNothingValues(false,currentStage,true);
        DropSpawner_EnemyDeath.AlterLiveNothingValues(true,currentStage,true);
        currentStage -=1;
        player.GetComponent<Player>().ChangeCurrentCriticalProbability(currentStage);
    }

    public override void ResetAllFromUpgrade(Transform player)
    {
        player.GetComponent<Player>().ChangeCurrentCriticalProbability(-1);
        DropSpawner_Timed.ResetAlteredTables();
        DropSpawner_EnemyDeath.ResetAlteredTables();
    }

}

public class SwordUpgrade : BaseUpgrade{

    
    public SwordUpgrade(){
        type = 4;
        currentStage = 0;
        possibleStages = 2;//meaning there are 3 stages
    }

    public override void AddActiveEffect(Transform player)
    {
        Debug.Log("UPGRADE OF TYPE " + type + "ON STAGE "+ currentStage + " ACTIVE");
        player.GetComponent<Player>().ChangeMeleeChargeSpeed(true);
    }

    public override void AddPassiveEffect(Transform player)
    {
        player.GetComponent<Player>().ChangeMeleeAttackLevel(0);
    }
    public override void RemovePassiveEffect(Transform player){
        player.GetComponent<Player>().ChangeMeleeAttackLevel(-1);
    }
    public override void RemoveActiveEffect(Transform player){
        Debug.Log("UPGRADE OF TYPE " + type + " IN-ACTIVE");
        player.GetComponent<Player>().ChangeMeleeChargeSpeed(false);
    }

    public override void Upgrade(Transform player){
        
        currentStage +=1;
    }

    public override void Downgrade(Transform player){
        //if we already at 0 this doesn't get called. it straight up goes to be removed

        currentStage -=1;
    }

    public override void ResetAllFromUpgrade(Transform player)
    {
        player.GetComponent<Player>().ChangeMeleeAttackLevel(-1);
    }
}

public class UpgradeController : MonoBehaviour
{

    
    private LinkedList<BaseUpgrade> deck;
    private LinkedListNode<BaseUpgrade> currentUpgrade;
    // Start is called before the first frame update


    //ui actions
    public event Action<int,BaseUpgrade> AddToCurrent;
    public event Action<int,BaseUpgrade> RemoveCurrent;
    public event Action<int,BaseUpgrade> SelectRight;
    public event Action<int,BaseUpgrade> SelectLeft;
    public event Action<BaseUpgrade> ReplaceCurrentCase;


     public int UpgradeLimit = 3;

    void Start()
    {
        deck = new LinkedList<BaseUpgrade>();
    }

    // Update is called once per frame
    public void AddToDeck(BaseUpgrade upgrade){
        //immidiately add to list or to the the right of what is current. then make the switch
        if(IsDeckEmpty()){
            deck.AddFirst(upgrade);
            currentUpgrade = deck.First;
            currentUpgrade.Value.AddPassiveEffect(transform);
            currentUpgrade.Value.AddActiveEffect(transform);
            AddToCurrent(deck.Count, upgrade);
            
        }else{
            if(deck.Count == UpgradeLimit){
                Debug.Log("UPGRADE SPACES FULL, INITIATING REPLACEMENT PROTOCOL");
                //this can ONLY happen to current space...:3
                
                //place delay

                //removecurrent
                currentUpgrade.Value.RemoveActiveEffect(transform);
                //currentUpgrade.Value.RemovePassiveEffect(transform);
                currentUpgrade.Value.ResetAllFromUpgrade(transform);
                //replace value
                currentUpgrade.Value = upgrade;
                currentUpgrade.Value.AddPassiveEffect(transform);
                currentUpgrade.Value.AddActiveEffect(transform);
                
                //call to UI
                ReplaceCurrentCase(upgrade);


                return;

            }
            deck.AddBefore(currentUpgrade,upgrade);
            
            upgrade.AddPassiveEffect(transform);
            AddToCurrent(deck.Count, upgrade);
            //ChangeCurrentRight(); //this would take care of active. but i guess we are not forcing a switch on entrance

        }
    }

    //MAKE IT WORK IN ACCORDANCE WITH THE UI ORDER PLEASE...I GUESS IT ALREADY WORKS???
    public void RemoveCurrentFromDeck(){
        Debug.Log("REMOVING FROM DECK");
        if(deck.Count == 1){
            currentUpgrade.Value.RemoveActiveEffect(transform);
            currentUpgrade.Value.RemovePassiveEffect(transform);
            RemoveCurrent(deck.Count, null);
            deck.RemoveLast();
            
        }else{

            var temp = new LinkedListNode<BaseUpgrade>(currentUpgrade.Value);
            currentUpgrade.Value.RemoveActiveEffect(transform);
            if(currentUpgrade.Previous!= null){
                temp = currentUpgrade.Previous;
            }else{
                temp = deck.Last;
            }
            RemoveCurrent(deck.Count, null);
            deck.Remove(currentUpgrade);
            currentUpgrade = temp;
            currentUpgrade.Value.AddActiveEffect(transform); //does switch to other on its own
        }
    }

    public void ChangeCurrentLeft(){
        if(IsDeckEmpty() || deck.Count == 1){
            Debug.Log("DECK EMPTY OR JUST ONE, CAN'T SWITCH");
            return;
        }

        if(currentUpgrade.Previous != null){
            //select previous one
            currentUpgrade.Value.RemoveActiveEffect(transform);
            currentUpgrade = currentUpgrade.Previous;
            currentUpgrade.Value.AddActiveEffect(transform);
        }else{
            //since the LL doesn't allow cycles we cycle to back on our own
            currentUpgrade.Value.RemoveActiveEffect(transform);
            currentUpgrade = deck.Last;
            currentUpgrade.Value.AddActiveEffect(transform);
        }
        SelectLeft(deck.Count,currentUpgrade.Value);
    }
    public void ChangeCurrentRight(){
        if(IsDeckEmpty() || deck.Count == 1){
            Debug.Log("DECK EMPTY OR JUST ONE CARD, CAN'T SWITCH");
            return;
        }

        if(currentUpgrade.Next != null){
            currentUpgrade.Value.RemoveActiveEffect(transform);
            currentUpgrade = currentUpgrade.Next;
            currentUpgrade.Value.AddActiveEffect(transform);
        }else{
            currentUpgrade.Value.RemoveActiveEffect(transform);
            currentUpgrade = deck.First;
            currentUpgrade.Value.AddActiveEffect(transform);
        }
        SelectRight(deck.Count, currentUpgrade.Value);
    }

    public bool IsDeckEmpty(){
        return (deck.Count == 0 ? true : false);
    }

    public void Upgrade(BaseUpgrade upgrade){
        //we do a search to see if we add to something already on stack...
        //if not on stack just call AddToDeck
        if(IsDeckEmpty()){
            AddToDeck(upgrade);
        }else{
            //search for a node that contains this upgrade type...if found increase its ability.
            foreach(BaseUpgrade ups in deck){
                if(upgrade.type == ups.type){
                    Debug.Log("SAME TYPE ENCOUTERED");
                    if(ups.currentStage == ups.possibleStages){
                        Debug.Log("ALREADY AT MAX LEVEL, DISREGARD");
                    }else{
                        //INCREASE POWER INT UPS
                        ups.Upgrade(transform); //we upgrade the one we have, dispose of the new one
                    }
                    //let UIknow that we can upgrade a card of type...!
                    //UI would have to do the search in its own base about this...
                    return;
                }
            }
            //if not found just addtodeck
            AddToDeck(upgrade);
        }
    }

    //called by player on Damage()
    public void Downgrade(){
        //check if we downgrading our existing card, no need to search already on pointer
        //if we don't then just call RemoveCurrentFromDeck
        if(IsDeckEmpty()){
            Debug.Log("nothing to downgrade");
            return;
        }

        if(currentUpgrade.Value.currentStage == 0){
            //REMOVAL
            RemoveCurrentFromDeck();
        }else{
            //downgrade
            currentUpgrade.Value.Downgrade(transform);
            //we should let UI know that a downgrade took place
        }
    }
    //public void RemoveByTimeOut(){
    //
    //    }

    public int[] ShareTypesCurrentlyInDeck(){
        if (IsDeckEmpty())
        {
            return null;
        }

        int[] result = {-1,-1,-1};

        int index = 0;
        
        foreach (var ups in deck)
        {
            
            if(ups.currentStage != ups.possibleStages){
                result[index] = ups.type;
            }
            index++;
        }
        return result;
        
    }
}
