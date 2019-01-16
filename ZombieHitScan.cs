using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHitScan : MonoBehaviour {

    ZombieScript zombieScript;
    Animator anim;
    PlayerController pController;


	// Use this for initialization
	void Start () {
        zombieScript = GetComponentInChildren<ZombieScript>();
        anim = gameObject.GetComponentInChildren<Animator>();
        zombieScript.SetHitBox(gameObject.GetComponent<BoxCollider2D>());
	}
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PGBodyCollider")
        {
            pController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            if(!anim.GetBool("Hurting"))
                zombieScript.Attack();
        }
    }
    */
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "PGBodyCollider")
        {
            pController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
            if (!anim.GetBool("Hurting") && anim.GetBool("Grounded") && !anim.GetBool("Attacking") && !pController.IsPlayerDead())
            {
                zombieScript.Attack();
            }
        }
    }
}
