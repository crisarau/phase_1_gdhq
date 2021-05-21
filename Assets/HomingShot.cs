using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingShot : MonoBehaviour
{
    public Transform target;

    [SerializeField]
    float speed;
    [SerializeField]
    float rotatespeed;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target==null){
            rb.rotation = Quaternion.LookRotation(rb.velocity, transform.up).z;
            rb.AddForce(transform.up * 0.5f);
        }else{
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            var turnAmount = Vector3.Cross(direction,transform.up);
            rb.angularVelocity = -turnAmount.z * rotatespeed;
            rb.velocity = transform.up * speed;
        }

        if(transform.position.y > 8.0f || transform.position.x > 8.0f || transform.position.y < -8.0f || transform.position.x < -8.0f){
            Destroy(this.gameObject);
        }
    }
}

