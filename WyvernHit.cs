using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernHit : MonoBehaviour {

    public BoxCollider2D boxCollider2D;
    PlayerController player;
    HealthScript healthScript;
    WyvernScript wyvernScript;

	// Use this for initialization
	void Start () {

        
        healthScript = GameObject.FindGameObjectWithTag("HealthScript").GetComponent<HealthScript>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
        wyvernScript = gameObject.GetComponentInChildren<WyvernScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PGBodyCollider")
        {
            if(!wyvernScript.IsWyvernShoved())
            player.Hurt(wyvernScript.facingRight);
        }
    }

}
