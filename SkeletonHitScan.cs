using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHitScan : MonoBehaviour {

    SkeletonScript skeletonScript;



	// Use this for initialization
	void Start () {
        skeletonScript = GetComponentInChildren<SkeletonScript>();
	}
	
	void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "PGBodyCollider")
        {
            skeletonScript.Attack();
        }
    }
}
