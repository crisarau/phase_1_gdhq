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
    private TextMeshProUGUI _ammoMax;
    [SerializeField]
    private TextMeshProUGUI _currentAmmo;
    private bool _isThrusterCooldown = false;

    [SerializeField]
    private GameObject _ammoUIPrefab;

    private IEnumerator _overHeatBlink;

    private Transform _ammoBar;

    // Start is called before the first frame update
    void Awake()
    {
        _player = FindObjectOfType<Player>();
        _thrusterFill = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        _ammoBar = transform.Find("AmmoBar");

        _overHeatBlink = OverHeatBlinking();
    }
    private void OnEnable() {
        _player.OnThrusterUpdate += ThrusterFill;
        _player.OnFireAmmoUpdate += AmmoUpdate;
        _player.OnShotEnqueue += AmmoEnqueueUpdate;
        _player.OnShotDequeue += AmmoDequeueUpdate;
    }

    void AmmoUpdate(int ammo, int max = -1){
        _currentAmmo.text = ammo.ToString("000");
        if(max>0){
            _ammoMax.text = "/ " + max.ToString("000");
        }
    }

    void AmmoEnqueueUpdate(bool critical){
        Debug.Log("Called!");
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
        }

    }

    void ThrusterFill(){
        _thrusterFill.fillAmount = 1.0f - (_player._currentThruster / _player.getMaxThruster());
        //if we hit 0...we are on cooldown mode...call on an IEnumerator
        if(_thrusterFill.fillAmount == 0.0f){
            _isThrusterCooldown = true;
            StartCoroutine(_overHeatBlink);   
        }
        //when we reach back to 1... we out of cooldown mode...stop the IEnumerator
        if(_isThrusterCooldown && _thrusterFill.fillAmount == 1.0f){
            _isThrusterCooldown = false;
            StopCoroutine(_overHeatBlink);
        }
    }
    IEnumerator OverHeatBlinking(){
        while(true){
            _overheatText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            _overheatText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25f);

        }
    }

}
