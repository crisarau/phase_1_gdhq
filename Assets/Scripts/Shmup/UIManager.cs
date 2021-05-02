﻿using System.Collections;
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
    private bool _isThrusterCooldown = false;

    private IEnumerator _overHeatBlink;

    // Start is called before the first frame update
    void Awake()
    {
        _player = FindObjectOfType<Player>();
        _thrusterFill = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        _overHeatBlink = OverHeatBlinking();
    }
    private void OnEnable() {
        _player.OnThrusterUpdate += ThrusterFill;
        
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
