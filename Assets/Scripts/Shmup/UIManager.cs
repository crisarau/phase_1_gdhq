using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private Player _player;
    private Image _thrusterFill;
    [SerializeField]
    private TextMeshProUGUI _overheatText;
    [SerializeField]
    private TextMeshProUGUI _dangerText;
    [SerializeField]
    private TextMeshProUGUI _ammoMax;
    [SerializeField]
    private TextMeshProUGUI _currentAmmo;
    

    [SerializeField]
    private GameObject _ammoUIPrefab;

    private UIShaker ammoBarShaker;
    private float maxThrusterFreq = 350;
    private UIShaker thrusterShaker;
    private bool _isThrusterCooldown = false;

    private UIShaker deckShaker;
    
    [SerializeField]
    private GameObject _healthBarUIPrefab;

    private IEnumerator _overHeatBlink;
    private IEnumerator _dangerBlink;

    private Transform _ammoBar;
    private Transform _lifeBar;

    //UpgradeDeckstuff
    [SerializeField]
    private GameObject _cardPrefab;
    private UpgradeController _playerUpgrades;
    private Transform UILiveUpgradesHolder;
    private List<Transform> UIUpgradeConstantPositions;

    private Image _meleeFill;


    // Start is called before the first frame update
    void Awake()
    {
        _player = FindObjectOfType<Player>();
        deckShaker = transform.Find("DeckHolder").GetComponent<UIShaker>();

        _thrusterFill = transform.Find("DeckHolder").transform.Find("DECK_LEFT").transform.Find("BoostBar").GetChild(0).GetComponent<Image>();
        thrusterShaker = transform.Find("DeckHolder").transform.Find("DECK_LEFT").transform.Find("BoostBar").GetChild(0).GetComponent<UIShaker>();

        _ammoBar = transform.Find("DeckHolder").transform.Find("DECK_RIGHT").transform.Find("AmmoBarHolder").GetChild(0);
        ammoBarShaker = _ammoBar.GetComponent<UIShaker>();

        _lifeBar = transform.Find("DeckHolder").transform.Find("DECK_LEFT").transform.Find("LifePanel").GetChild(0);

        _overHeatBlink = OverHeatBlinking();
        _dangerBlink = DangerBlinking();

        //upgrade stuff
        _playerUpgrades = FindObjectOfType<UpgradeController>();
        UILiveUpgradesHolder = transform.Find("DeckHolder").Find("UPGRADE_PANEL").Find("UPGRADES_AT_PLAY");

        UIUpgradeConstantPositions = new List<Transform>();
        var tempcount = transform.Find("DeckHolder").Find("UPGRADE_PANEL").Find("STATIC_POSITIONS");
        for(int i =0; i <  tempcount.childCount; i++){
            UIUpgradeConstantPositions.Add(tempcount.GetChild(i));
        }
        //UIUpgradeConstantPositions = transform.Find("DeckHolder").Find("STATIC_POSITIONS");

        _meleeFill = transform.Find("DeckHolder").transform.Find("DECK_LEFT").transform.Find("MeleeBar").GetChild(0).GetComponent<Image>();
    }
    private void OnEnable() {
        _player.OnThrusterUpdate += ThrusterFill;
        _player.OnFireAmmoUpdate += AmmoUpdate;
        _player.OnShotEnqueue += AmmoEnqueueUpdate;
        _player.OnShotDequeue += AmmoDequeueUpdate;
        _player.OnHealthUpdate += HealthUpdate;
        _player.OnMeleeAttackUpdate += MeleeAttackUpdate;

        //upgrade stuff
        _playerUpgrades.AddToCurrent += AddToActivePosition;
        _playerUpgrades.RemoveCurrent += RemoveFromActivePosition;
        _playerUpgrades.SelectLeft += SwipeLeft;
        _playerUpgrades.SelectRight += SwipeRight;
        _playerUpgrades.ReplaceCurrentCase += ReplaceCurrent;

    }

    private void OnDisable() {
        _player.OnThrusterUpdate -= ThrusterFill;
        _player.OnFireAmmoUpdate -= AmmoUpdate;
        _player.OnShotEnqueue -= AmmoEnqueueUpdate;
        _player.OnShotDequeue -= AmmoDequeueUpdate;
        _player.OnHealthUpdate -= HealthUpdate;
        _player.OnMeleeAttackUpdate -= MeleeAttackUpdate;

        //upgrade stuff
        _playerUpgrades.AddToCurrent -= AddToActivePosition;
        _playerUpgrades.RemoveCurrent -= RemoveFromActivePosition;
        _playerUpgrades.SelectLeft -= SwipeLeft;
        _playerUpgrades.SelectRight -= SwipeRight;
        _playerUpgrades.ReplaceCurrentCase -= ReplaceCurrent;
    }

    void AmmoUpdate(int ammo, int max = -1){
        _currentAmmo.text = ammo.ToString("000");
        if(max>0){
            _ammoMax.text = "/ " + max.ToString("000");
        }
    }

    void HealthUpdate(int health){
        //if took damage
        if(health < _lifeBar.childCount+1){

            deckShaker.Shake(1f);
            Camera.main.GetComponent<CameraShaker>().Shake(1f); //optimize this
            if(health == 0){
                StopCoroutine(_dangerBlink);
                _dangerText.gameObject.SetActive(false);
                return;
            }
            Destroy(_lifeBar.GetChild(_lifeBar.childCount-1).gameObject);
            //if(_lifeBar.childCount == 0){
            if(health == 1){
                Debug.Log("no more children...starting corrotuine");
                StartCoroutine(_dangerBlink);
            }
        }else{
            //recovered health
            if(_lifeBar.childCount == 0){
                StopCoroutine(_dangerBlink);
                _dangerText.gameObject.SetActive(false);
            }
            GameObject temp = Instantiate(_healthBarUIPrefab);
            temp.transform.parent = _lifeBar;
            //temp.transform.SetSiblingIndex(_lifeBar.childCount-1);
        }
    
    }

    void AmmoEnqueueUpdate(bool critical){
        //Debug.Log("Called!");
        GameObject temp = Instantiate(_ammoUIPrefab);
        if(critical){
            temp.GetComponent<Image>().color = Color.red;
        }
        temp.transform.parent = _ammoBar;
        //if(_ammoBar.childCount != 0){
        temp.transform.SetSiblingIndex(_ammoBar.childCount-1);
        //temp.transform.SetSiblingIndex(0);
        //}
    }

    void AmmoDequeueUpdate(bool critical){
        Destroy(_ammoBar.GetChild(0).gameObject);
        //Destroy(_ammoBar.GetChild(_ammoBar.childCount-1).gameObject);
        if(critical){
            //do some animation thingy. do that thing that keeps it alive until animation done
            ammoBarShaker.Shake(1f);
        }

    }
    //up = true, down = false
    void ThrusterFill(bool direction){
        _thrusterFill.fillAmount = 1.0f - (_player._currentThruster / _player.getMaxThruster());
        thrusterShaker.ChangeFrequencies(maxThrusterFreq*(1f-_thrusterFill.fillAmount));
        
        //if we hit 0...we are on cooldown mode...call on an IEnumerator
        if(_thrusterFill.fillAmount == 0.0f){
            _isThrusterCooldown = true;
            StartCoroutine(_overHeatBlink);   
        }

        if (direction){
            thrusterShaker.Shake(0.1f);
        }
            

        //when we reach back to 1... we out of cooldown mode...stop the IEnumerator
        if(_isThrusterCooldown && _thrusterFill.fillAmount == 1.0f){
            _isThrusterCooldown = false;
            StopCoroutine(_overHeatBlink);
            _overheatText.gameObject.SetActive(false);
            thrusterShaker.ChangeFrequencies(0);
        }
    }


    void MeleeAttackUpdate(float fill){
        _meleeFill.fillAmount = fill;
    }
    IEnumerator OverHeatBlinking(){
        while(true){
            _overheatText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            _overheatText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);

        }
    }


    IEnumerator DangerBlinking(){
        while(true){
            _dangerText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            _dangerText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);

        }
    }

    void AddToActivePosition(int count, BaseUpgrade upgrade){
        switch (count)
        {
            case 1:
                //var temp = ;
                TemporaryColorPickerBasedOnTypeOfUpgrade(Instantiate(_cardPrefab,UIUpgradeConstantPositions[2].position, Quaternion.identity, UILiveUpgradesHolder), upgrade.type,false);       
                break;
            case 2:
                //var temp = Instantiate(_cardPrefab,UIUpgradeConstantPositions[1].position, Quaternion.identity, UILiveUpgradesHolder);
                TemporaryColorPickerBasedOnTypeOfUpgrade(Instantiate(_cardPrefab,UIUpgradeConstantPositions[1].position, Quaternion.identity, UILiveUpgradesHolder), upgrade.type,false);
                
                break;
            case 3:
                //var temp = Instantiate(_cardPrefab,UIUpgradeConstantPositions[1].position, Quaternion.identity, UILiveUpgradesHolder);
                TemporaryColorPickerBasedOnTypeOfUpgrade(Instantiate(_cardPrefab,UIUpgradeConstantPositions[3].position, Quaternion.identity, UILiveUpgradesHolder), upgrade.type, true);
                break;
            default:
                break;
        }
    }

    GameObject TemporaryColorPickerBasedOnTypeOfUpgrade(GameObject toSet, int upgradeType, bool setFront){
        switch (upgradeType)
        {
            case 0:
                toSet.GetComponent<Image>().color = new Color32(252, 3, 3,255);
                break;
            case 1:
                toSet.GetComponent<Image>().color = new Color32(0, 37, 219,255);
                break;
            case 2:
                toSet.GetComponent<Image>().color = new Color32(127, 50, 168, 255);
                break;
            case 3:
                toSet.GetComponent<Image>().color = new Color32(25, 192, 25, 255);
                break;
            case 4:
                toSet.GetComponent<Image>().color = new Color32(7, 113, 130, 255);
                break;
            default:
                toSet.GetComponent<Image>().color = new Color32(252, 233, 3,255);
                break;
        }
        if(!setFront){
            toSet.transform.SetAsFirstSibling();
        }
        return toSet;
    }
    void RemoveFromActivePosition(int count, BaseUpgrade upgrade){
        //get current the hell out of there. or maybe we could use a curve for this? idk lol
        Debug.Log("UI SAYS TO REMOVE");
        if(count == 2){

            var tempToRemove = UILiveUpgradesHolder.GetChild(1).gameObject;
            LeanTween.move(tempToRemove,UIUpgradeConstantPositions[2].position + new Vector3(0,100,0), 0.08f).setOnComplete(() => DestroyUpgrade(tempToRemove)); //Should I cache this? idk if it will remain teh same
        

            //move back card to current
            LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[2].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(0,true));
        }else if(count == 3){

            var tempToRemove = UILiveUpgradesHolder.GetChild(1).gameObject;
            LeanTween.move(tempToRemove,UIUpgradeConstantPositions[2].position + new Vector3(0,100,0), 0.08f).setOnComplete(() => DestroyUpgrade(tempToRemove)); //Should I cache this? idk if it will remain teh same
        
            //move front card to current position
            LeanTween.move(UILiveUpgradesHolder.GetChild(2).gameObject, UIUpgradeConstantPositions[2].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(2,false));
        
        }else if(count == 1){
            var tempToRemove = UILiveUpgradesHolder.GetChild(0).gameObject;
            LeanTween.move(tempToRemove,UIUpgradeConstantPositions[2].position + new Vector3(0,100,0), 0.08f).setOnComplete(() => DestroyUpgrade(tempToRemove)); //Should I cache this? idk if it will remain teh same
        
        }
    }

    void SwipeLeft(int count, BaseUpgrade upgrade){
        //tbh we only move if we have 2 or 3 active...
        if(count == 2){
            LeanTween.move(UILiveUpgradesHolder.GetChild(1).gameObject, UIUpgradeConstantPositions[1].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(1,false));

            LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[0].position, 0.10f).setOnComplete(()=> OnUpgradeOutOfView(count,false));
        }else if(count == 3){
            
            LeanTween.move(UILiveUpgradesHolder.GetChild(1).gameObject, UIUpgradeConstantPositions[1].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(1,false));
            LeanTween.move(UILiveUpgradesHolder.GetChild(2).gameObject, UIUpgradeConstantPositions[2].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(2,false));

            LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[0].position, 0.10f).setOnComplete(()=> OnUpgradeOutOfView(count,false));
        }

    }
    void SwipeRight(int count, BaseUpgrade upgrade){
        if(count == 2){
            LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[2].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(0,true));

            LeanTween.move(UILiveUpgradesHolder.GetChild(1).gameObject, UIUpgradeConstantPositions[3].position, 0.10f).setOnComplete(()=> OnUpgradeOutOfView(count,true));
        }else if(count == 3){
            
            LeanTween.move(UILiveUpgradesHolder.GetChild(1).gameObject, UIUpgradeConstantPositions[3].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(1,true));
            LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[2].position, 0.08f).setOnComplete(() => OnFinishingUpgradeMove(0,true));


            LeanTween.move(UILiveUpgradesHolder.GetChild(2).gameObject, UIUpgradeConstantPositions[3].position - new Vector3(-1,-1,0), 0.10f).setOnComplete(()=> OnUpgradeOutOfView(count,true));
        }
    }

    void OnUpgradeOutOfView(int currentCount, bool direction){
        //direction, false left true right

        
        
        if(direction){
            //place out of view
            UILiveUpgradesHolder.GetChild(0).position = Vector3.zero;
            
            if(currentCount == 2){
                LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[1].position, 0.08f); //should i let someone know about this???
            }else if(currentCount == 3){
                LeanTween.move(UILiveUpgradesHolder.GetChild(0).gameObject, UIUpgradeConstantPositions[0].position, 0.08f); //should i let someone know about this???    
            }
        }else{
            //place out of view
            UILiveUpgradesHolder.GetChild(UILiveUpgradesHolder.childCount-1).position = Vector3.zero;
            //then make it go to it's position.
            if(currentCount == 2){
                LeanTween.move(UILiveUpgradesHolder.GetChild(UILiveUpgradesHolder.childCount-1).gameObject, UIUpgradeConstantPositions[2].position, 0.08f); //should i let someone know about this???
            }else if(currentCount == 3){
                LeanTween.move(UILiveUpgradesHolder.GetChild(UILiveUpgradesHolder.childCount-1).gameObject, UIUpgradeConstantPositions[3].position, 0.08f); //should i let someone know about this???    
            }
        }
        
    }
    void OnFinishingUpgradeMove(int currentIndex, bool direction){
        //direction, false left true right

        if(direction){
            //change the order in child hierarchy
            if(currentIndex==0){
                UILiveUpgradesHolder.GetChild(currentIndex).SetSiblingIndex(1);

            }else if(currentIndex==1){
                UILiveUpgradesHolder.GetChild(currentIndex).SetSiblingIndex(2);
            }
        }else{
            //change the order in child hierarchy
            if(currentIndex==1){
                UILiveUpgradesHolder.GetChild(currentIndex).SetSiblingIndex(0);

            }else if(currentIndex==2){
                UILiveUpgradesHolder.GetChild(currentIndex).SetSiblingIndex(1);
            }
        }
    }
    void DestroyUpgrade(GameObject card){
        //gets called when we dismiss a card entirely.
        Destroy(card);
        Debug.Log("DESTROYED CARD");
    }
    void ReplaceCurrent(BaseUpgrade upgrade){
        //current flies out
        var tempToRemove = UILiveUpgradesHolder.GetChild(1).gameObject;
        LeanTween.move(tempToRemove,UIUpgradeConstantPositions[1].position + new Vector3(0,100,0), 0.08f).setOnComplete(() => DestroyUpgrade(tempToRemove)); //Should I cache this? idk if it will remain teh same

        //do something with the return...maybe animation idk
        var tempToAdd = TemporaryColorPickerBasedOnTypeOfUpgrade(Instantiate(_cardPrefab,UIUpgradeConstantPositions[1].position, Quaternion.identity, UILiveUpgradesHolder), upgrade.type,false);
        tempToAdd.transform.SetSiblingIndex(1);
        LeanTween.move(tempToAdd,UIUpgradeConstantPositions[1].position, 0.08f); //Should I cache this? idk if it will remain teh same

        
    }
}
