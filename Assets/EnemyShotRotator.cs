using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotRotator : MonoBehaviour
{
    [SerializeField]
    private bool autoAim;
    [SerializeField]
    private Transform aimTarget;

    [SerializeField]
    private float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(autoAim){
            Vector2 direction = (aimTarget.position - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;//because direction is on world spaceS
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(0f,0f, Mathf.Lerp(transform.rotation.eulerAngles.z, Quaternion.LookRotation(direction, Vector3.forward).z, Time.deltaTime * rotationSpeed));
        }
    }

    public void SetAimTarget(Transform target){
        aimTarget = target;
    }
    public void SetAutoAim(bool state){
        autoAim = state;
    }
}
