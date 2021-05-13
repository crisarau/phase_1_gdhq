using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    [SerializeField]
    private int powerupID;
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
                    default:
                        break;
                }
            }
            Destroy(this.gameObject);
        }    
    }
    
}
