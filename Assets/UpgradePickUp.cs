using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickUp : MonoBehaviour
{

    [SerializeField]
    private int UpgradeID;

    [SerializeField]
    private Player playerRef;
    
    [SerializeField]
    private float speed;
    

    BaseUpgrade upgrade;

    bool magnetActive;
    
    // Start is called before the first frame update
    void Start()
    {
        magnetActive = false;
        playerRef = GameObject.Find("Player").GetComponent<Player>();
        playerRef.PickUpsMagnetActivation += ChangeMagnetStatus;
        
        switch(UpgradeID){
            case 0:
                upgrade = new MultiShotUpgrade();
                break;
            case 1:
                upgrade = new MirrorShotUpgrade();
                break;
            case 2:
                upgrade = new HomingShotUpgrade();
                break;
            case 3:
                upgrade = new LuckyUpgrade();
                break;
            case 4:
                upgrade = new SwordUpgrade();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            //access player's UpgradeController
            var temp = other.transform.GetComponent<UpgradeController>();
            if(temp != null){
                temp.Upgrade(upgrade);
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
    //because i never know how many picksUps available...i could do a call to all with a tag OR a call of type but this is easier lol
    //private void OnEnable() {
    //    playerRef.PickUpsMagnetActivation += ChangeMagnetStatus;
    //}
    //had to get rid of OnEnable because it ran before player existing... and i couldn't assign reference to prefab for some reason
    //documentation says this is called on destroy too!
    private void OnDisable() {
        playerRef.PickUpsMagnetActivation -= ChangeMagnetStatus;
    }
}
