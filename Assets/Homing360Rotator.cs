using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing360Rotator : MonoBehaviour
{
    [SerializeField]float rotZ;
    float startingRotZ;
    bool spinned;
    public float rotationSpeed;
    public bool Clockwise;

    [SerializeField] GameObject rayChild;

    void Start()
    {
        startingRotZ = rotZ;
        spinned = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Clockwise){
            rotZ += -Time.deltaTime * rotationSpeed;
        }else{
            rotZ += Time.deltaTime * rotationSpeed;
        }
        transform.rotation = Quaternion.Euler(0,0,rotZ);
        if(rotZ%360 < startingRotZ && !spinned){
            spinned = true;
        }
        if(rotZ%360 >= startingRotZ && spinned){
            Debug.Log("Did first full spin!");
            rayChild.SetActive(false);
            this.enabled = false;
        } 
    }
}
