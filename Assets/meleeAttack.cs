using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeAttack : MonoBehaviour
{
    private BoxCollider2D col;
    private Animator meeleeAnim;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        meeleeAnim = GetComponent<Animator>();
        col.enabled = false;
    }

    public void Attack(){
        if(canAttack()){
            meeleeAnim.SetBool("sword",true);
        }
    }

    public bool canAttack(){
        return !meeleeAnim.GetCurrentAnimatorStateInfo(0).IsName("sword_slash_01");
    }

    public void EndAttack(){
        meeleeAnim.SetBool("sword",false);
    }

    public void EnableCol(){
        col.enabled = true;
    }
    public void DisableCol(){
        col.enabled = false;
    }
}
