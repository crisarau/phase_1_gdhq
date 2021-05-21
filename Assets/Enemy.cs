using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;
    Player _player;
    void Start()
    {
        //_player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate( Vector3.up * -1.0f * _speed * Time.deltaTime);
        
        if(transform.position.y < -4.5f) {
            transform.position = new Vector3(Random.Range(-5.5f, 5.5f),6.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            //damage player

            //nullchecking in case there is no component active.
            Player player = other.transform.GetComponent<Player>();
            if(player != null){
                player.Damage();
            }
            
            Destroy(this.gameObject);
            
        }
        if(other.tag == "Laser"){
            Destroy(other.gameObject);
            _speed = 0;
            Destroy(this.gameObject);
            
        }
        
    }
}
