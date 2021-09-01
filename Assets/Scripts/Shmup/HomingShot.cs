using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingShot : MonoBehaviour, IProjectile
{
    public Transform target;

    [SerializeField]
    float speed;

    [SerializeField]
    private int _currentHealth;

    [SerializeField]
    private float lifetime;
    private float timerToExit;

    [SerializeField]
    float rotatespeed;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timerToExit = lifetime+Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time >= timerToExit){
            DegradeProjectile(5);
        }

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

    public void DegradeProjectile(int degradation){
        _currentHealth -= degradation;
        if(_currentHealth <= 0 ){
            Destroy(this.gameObject);
        }
    }
}

