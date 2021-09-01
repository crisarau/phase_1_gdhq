using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaBlastProjectile : MonoBehaviour, IProjectile
{

    [SerializeField]
    private float _speed = 8.0f;
    // Start is called before the first frame update
    [SerializeField]
    private int _currentHealth;

    [SerializeField]
    private Vector2 moveDirection;

    void Awake()
    {
        SetMoveDirection(Vector3.up);
    }
    public void SetMoveDirection(Vector2 dir){
        moveDirection = dir;
        //transform.rotation = Quaternion.LookRotation(dir);

        //transform.rotation *= Quaternion.FromToRotation(transform.right, dir);


    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection *  _speed * Time.deltaTime);

        if(transform.position.y > 8.0f || transform.position.x > 8.0f || transform.position.y < -8.0f || transform.position.x < -8.0f){
            if(transform.parent != null){
                //if there is a parent...destroy the parent which also destroys the child automatically.
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    
    public void DegradeProjectile(int degradation){
        _currentHealth -= degradation;
        if(_currentHealth <= 0 ){
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.tag == "PlayerAttack"){
            DegradeProjectile(1);
            IProjectile proj = other.transform.GetComponent<IProjectile>();
            if(proj != null){
                proj.DegradeProjectile(1);
            }
        }
    }


}
