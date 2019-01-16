using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDestroy : MonoBehaviour {

    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void SkeletonDest()
    {
        DestroyObject(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            if (!gameObject.GetComponentInChildren<SkeletonScript>().IsWaitingToStart)
            {
                anim.SetBool("Active", true);
            }
        }
    }
}
