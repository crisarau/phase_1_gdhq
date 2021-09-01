using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvasiveColliderTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Untagged"){
            return;
        }

        //Debug.Log("COLLISION FROM THE EVASION HURTBOX");
        if(other.tag == "PlayerAttack"){
            Debug.Log("is evasive collider trigger called?");
            transform.parent.gameObject.GetComponent<EnemyController>().ChangeEvasiveAbilityStatus(true, other);
        }
    }
}
