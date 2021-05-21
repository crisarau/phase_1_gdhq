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
    }

    public void ResizeTargetAmount(int resize){
        if (resize == 0){
            return;
        }
        Array.Resize(ref targetsOverlap, resize*2);
        Array.Clear(targetsOverlap,0,targetsOverlap.Length);
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void Fire(){
        finalTargets.Clear();
        Debug.Log("ClearingFinalTarget!"+ finalTargets.Count);
        int numberOfHits = Physics2D.OverlapCollider(fullScreen,layerMask,targetsOverlap);
        if(numberOfHits != 0){
            Debug.Log("this many hits..."+ numberOfHits);
            for(int i = 0; i<targetsOverlap.Length;i++){
                if(targetsOverlap[i]){
                    if(!finalTargets.Contains(targetsOverlap[i].transform)){
                        finalTargets.Add(targetsOverlap[i].transform);
                        GameObject icon = Instantiate(targetUIPrefab, Camera.main.WorldToScreenPoint(targetsOverlap[i].transform.position),Quaternion.identity,targetUIPrefabParent.transform);
                        icon.GetComponent<TargetUI>().target = targetsOverlap[i].transform;
                    }
                }
            }
            Array.Clear(targetsOverlap,0,2);
        }
        if(finalTargets.Count != 0 && finalTargets.Count != targetsOverlap.Length){
            //we need to repeat.
            int i = finalTargets.Count;
            while(i != targetsOverlap.Length){
                finalTargets.Add(finalTargets[UnityEngine.Random.Range(0,finalTargets.Count-1)]);
                i++;
            }
        }

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
