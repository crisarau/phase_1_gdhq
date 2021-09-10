using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    [SerializeField]
    private int powerupID;

    [SerializeField]
    private Player playerRef;
    
    [SerializeField]
    private float speed;
    bool magnetActive;

    private void Start() {
        magnetActive = false;
        playerRef = GameObject.Find("Player").GetComponent<Player>();
        playerRef.PickUpsMagnetActivation += ChangeMagnetStatus;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            Player player = other.transform.GetComponent<Player>();

            if(player != null){
                switch(powerupID){
                    case 0:
                        player.AddAmmo(50);
                        break;
                    case 1:
                        //Debug.Log("COLLECTE SPEED");
                        player.Heal();
                        break;
                    case 2:
                        player.Damage();
                        break;
                    case 3:
                        //player.MirrorShotActive();
                        break;
                    case 4:
                        //player.ActivateShots(1,2);
                        break;
                    default:
                        break;
                }
            }
            Destroy(this.gameObject);
        }    
    }

    private void Update() {

        if(transform.position.y < -9){
            //went too far down time to die
            Destroy(this.gameObject);
        }

        if(magnetActive){
            transform.position += ((playerRef.transform.position - transform.position ).normalized * (speed * 2.5f)* Time.deltaTime);
            
        }else{
            transform.Translate(-Vector3.up * speed * Time.deltaTime, Space.World);
        }
    }
    void ChangeMagnetStatus(bool state){
        magnetActive = state;
    }
    private void OnDisable() {
        playerRef.PickUpsMagnetActivation -= ChangeMagnetStatus;
    }
    
}
