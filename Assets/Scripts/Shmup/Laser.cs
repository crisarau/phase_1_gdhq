using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour, IProjectile
{
    [SerializeField]
    private float _speed = 8.0f;
    // Start is called before the first frame update
    [SerializeField]
    private int _currentHealth;
    

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up *  _speed * Time.deltaTime);

        if(transform.position.y > 8.0f || transform.position.x > 8.0f || transform.position.y < -8.0f || transform.position.x < -8.0f){
            if(transform.parent != null){
                //if there is a parent...destroy the parent which also destroys the child automatically.
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    
    public void DegradeProjectile(int degradation){
        _currentHealth -= 1;
        if(_currentHealth <= 0 ){
            Destroy(this.gameObject);
        }
    }
}
