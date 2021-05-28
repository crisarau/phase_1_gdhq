using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing360Detector : MonoBehaviour
{
    public Vector3 direction = Vector3.up;
    public RaycastHit2D hit;
    public float MaxDistance = 10;
    public LayerMask layerMask;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * MaxDistance, Color.green);
        hit = Physics2D.Raycast(transform.position,transform.TransformDirection(Vector2.up),MaxDistance,layerMask);
        if(hit){
            Debug.DrawRay(transform.position,transform.TransformDirection(Vector2.up) * MaxDistance, Color.red);
            print(hit.transform.name);
        }
    }

    
}
