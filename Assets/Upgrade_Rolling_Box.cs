using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Rolling_Box : MonoBehaviour
{

    //how does this work??
    // once it gets hit by player...it will call to deck to add. and destroy itself

    [SerializeField]
    private float switchingDelay;
    private float switchTime;
    [SerializeField]
    private int typeSelected;

    //[SerializeField]
    //private List<Sprite> sprites;


    private SpriteRenderer currentSprite;

    private int limit;

    private void Start() {
        currentSprite = transform.GetComponent<SpriteRenderer>();
        limit = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= switchTime){
            //time to change
            if(limit <= typeSelected){
                //went over limit
                SetSelection(0);
            }else{
                SetSelection(typeSelected + 1);
            }
            SetValue(typeSelected);

            switchTime = Time.time + switchingDelay;
        }
    }
    public void SetValue(int val){
        //change color of sprite to match the list
        //for now just changing colors
        TemporaryColorPickerBasedOnTypeOfUpgrade(val);
    }

    //u do the checking for  being inrange of 0 -4 i don't feel like doing it lol
    public void SetSelection(int val){
        typeSelected = val;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){

            BaseUpgrade upgrade;
            switch(typeSelected){
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
                    upgrade = new MultiShotUpgrade();
                    break;
            }

            //access player's UpgradeController
            var temp = other.transform.GetComponent<UpgradeController>();
            if(temp != null){
                temp.Upgrade(upgrade);
            }
            Destroy(this.gameObject);
        }    
    }


    void TemporaryColorPickerBasedOnTypeOfUpgrade(int colorselect){
        switch (colorselect)
        {
            case 0:
                currentSprite.color = new Color32(252, 3, 3,255);
                break;
            case 1:
                currentSprite.color = new Color32(0, 37, 219,255);
                break;
            case 2:
                currentSprite.color = new Color32(127, 50, 168, 255);
                break;
            case 3:
                currentSprite.color = new Color32(25, 192, 25, 255);
                break;
            case 4:
                currentSprite.color = new Color32(7, 113, 130, 255);
                break;
            default:
                currentSprite.color = new Color32(252, 233, 3,255);
                break;
        }
    }
}
