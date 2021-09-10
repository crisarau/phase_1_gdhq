using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HomingOverlapTarget : MonoBehaviour
{

    [SerializeField]
    Collider2D fullScreen;
    [SerializeField]
    Collider2D[] targetsOverlap;
    [SerializeField]List<Transform> finalTargets = new List<Transform>();

    [SerializeField]GameObject targetUIPrefab;
    [SerializeField]Canvas targetUIPrefabParent;
    [SerializeField]GameObject HomingShotPrefab;

    public ContactFilter2D layerMask;
    // Start is called before the first frame update
    void Start()
    {
        targetsOverlap = new Collider2D[2];
        fullScreen.enabled = false;
    }

    public void ResizeTargetAmount(int resize){
        if (resize == 0){
            return;
        }
        Array.Resize(ref targetsOverlap, resize);
        Array.Clear(targetsOverlap,0,targetsOverlap.Length);
    }

    // Update is called once per frame
    //void Update()
    //{
        //if(Input.GetKey(KeyCode.H)){
        //    Debug.Log("HomingShot!");
        //    int numberOfHits = Physics2D.OverlapCircleNonAlloc(transform.position, 10f, targetsOverlap,layerMask);
        //    if(numberOfHits != 0){
        //        Debug.Log("this many hits..."+ numberOfHits);
        //        for(int i = 0; i<2;i++){
        //            if(targetsOverlap[i]){
        //                if(!finalTargets.Contains(targetsOverlap[i].transform)){
        //                    finalTargets.Add(targetsOverlap[i].transform);
        //                }
        //            }
        //        }
        //        Array.Clear(targetsOverlap,0,2);
        //    }
        //}    
    //}

    public void Fire(){
        
        //clear the target List.
        finalTargets.Clear();
        //Debug.Log("ClearingFinalTarget!"+ finalTargets.Count);
        
        //enable the collider
        fullScreen.enabled = true;

        //get number of hits, all collider2ds added to the targetOverlap array
        int numberOfHits = Physics2D.OverlapCollider(fullScreen,layerMask,targetsOverlap);
        if(numberOfHits != 0){
            Debug.Log("this many hits..."+ numberOfHits);
            for(int i = 0; i<targetsOverlap.Length;i++){
                if(targetsOverlap[i]){ //check if not null
                    if(!finalTargets.Contains(targetsOverlap[i].transform)){ //if finalTargets doesn't contain one of our newly found targets
                        finalTargets.Add(targetsOverlap[i].transform); //we add it and give it an UI icon
                        GameObject icon = Instantiate(targetUIPrefab, Camera.main.WorldToScreenPoint(targetsOverlap[i].transform.position),Quaternion.identity,targetUIPrefabParent.transform);
                        icon.GetComponent<TargetUI>().target = targetsOverlap[i].transform;
                    }
                }
            }
            //clear the target array...Should we check if it even got one?
            Array.Clear(targetsOverlap,0,targetsOverlap.Length-1);
        } //should i cancel if we get no hits?

        //turn off teh collider
        fullScreen.enabled = false;
        //if there are spaces for targets left over we repeat the targeting to pick one of the ones we have for another homing missile
        if(targetsOverlap.Length > 1 && finalTargets.Count != 0 && finalTargets.Count != targetsOverlap.Length){
            //we need to repeat.
            int i = finalTargets.Count;
            while(i != targetsOverlap.Length){
                finalTargets.Add(finalTargets[UnityEngine.Random.Range(0,finalTargets.Count-1)]);
                i++;
            }
        }
        //go through all the targets and instantiate the homing missiles and sets a target for them all.
        foreach(Transform shot in finalTargets){
            GameObject temp = Instantiate(HomingShotPrefab,transform.position,Quaternion.identity);
            temp.GetComponent<HomingShot>().target = shot;
        }
    }
    void OnDrawGizmos(){
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, 10f);
        //Gizmos.DrawCube(new Vector3(0,transform.position.y,transform.position.z), new Vector3(12,6,1)); //full screen coverage.. moves with y
    }



}
