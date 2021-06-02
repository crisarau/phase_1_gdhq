using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterExplosion : MonoBehaviour
{
    [SerializeField]
    private float _timeStartup = 0.0833f;
    [SerializeField]
    private float _timeActive = 0.5f;
    [SerializeField]
    private float _timeToDestruction = 1f;

    private float _timeToDeactivateCollider;
    private float _timeToActivateCollider;
    
    private BoxCollider2D col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        _timeToActivateCollider = Time.time + _timeStartup;
        _timeToDeactivateCollider = Time.time + _timeActive;


        //play animation.


        Destroy(this.gameObject, _timeToDestruction);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= _timeToActivateCollider){
            col.enabled = true;
        }
        if((col.enabled) && Time.time >= _timeToDeactivateCollider){
            col.enabled = false;
        }
    }
}
