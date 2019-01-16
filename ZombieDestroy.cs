using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDestroy : MonoBehaviour {

    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void ZombieDest () {
            DestroyObject(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            anim.SetBool("Activated", true);
        }
    }


}
