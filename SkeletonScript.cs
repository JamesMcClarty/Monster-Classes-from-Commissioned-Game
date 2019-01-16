using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : MonoBehaviour, IMonster
{
    
    int points = 0;
    float speed = 1;
    float invincibleTimer = 0;
    private HealthScript healthScript;
    private Animator anim;
    private Rigidbody2D rigidBody;
    private PlayerController pController;
    private Transform player;
    private BoxCollider2D box;
    private SkeletonDestroy skeletonDestroy;
    private bool attacking = false;
    private bool deadBool;
    private Quaternion rightSide;
    private Quaternion leftSide;
    private Vector3 lastKnownPosition;
    private SkinnedMeshRenderer[] skeletonRenderer;
    private Material skeletonMaterial;
    private Transform preFabTransform;
    

    public GameObject[] skeletonParts;
    public Transform[] skeletonPartTransform;

    public int health; 
    public bool shoved;
    public bool IsWaitingToStart;
    public Color hitColor;
    public GameObject hitSpark;
    public BoxCollider2D hitBox;
    public Transform wallBounce;
    public Transform groundCheck;
    public LayerMask surface;
    public bool hitTarget;
    public bool attackHit = false;
    public bool facingRight = true;
    [SerializeField]
    public RaycastHit2D[] gameObjectList;
    public LimbScript[] limbs;
    public GameObject arrow;
    public Transform arrowSpawn;

    public void DamageHealth() { health--; }
    public float GetSpeed() { return speed; }
    public int ReturnPoints() { return points; }

    public void Idle()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Walking", false);
    }

    public void Attack()
    {

        anim.SetBool("Idle", false);
        anim.SetBool("Walking", false);
        anim.SetBool("Attacking", true);
        attacking = true;

    }

    public void Move()
    {

        anim.SetBool("Idle", false);
        anim.SetBool("Walking", true);
        if (!shoved && anim.GetBool("Grounded"))
        {
            if (facingRight)
            {
                rigidBody.velocity = new Vector2(GetSpeed(), rigidBody.velocity.y);
            }
            else
            {
                rigidBody.velocity = new Vector2(-GetSpeed(), rigidBody.velocity.y);
            }
        }

    }

    public void TakeDamage()
    {
        if (invincibleTimer <= 0)
        {

            invincibleTimer = 0.7f;
            Instantiate(hitSpark, new Vector3(lastKnownPosition.x, lastKnownPosition.y + 2, -2), Quaternion.identity);
            if (player.position.x <= gameObject.GetComponentInParent<Transform>().position.x)
            {

                facingRight = false;

            }
            else
            {
                facingRight = true;
            }
            AdjustSides();
            anim.SetBool("Idle", false);
            anim.SetBool("Walking", false);
            anim.SetBool("Attacking", false);
            DamageHealth();
            if (health <= 0)
            {
                Die();
            }
            else
            {
                anim.SetTrigger("Hurting");
                foreach (SkinnedMeshRenderer skin in skeletonRenderer)
                {
                    skin.material.color = hitColor;
                }
            }
            

        }

    }

    public void GetShoved()
    {
        Debug.Log("Shoved");
        shoved = true;
        anim.SetBool("Attacking", false);
        anim.SetBool("Idle", false);
        AdjustSides();
        anim.SetBool("Grounded", false);
        anim.SetTrigger("Shoved");
        anim.SetBool("Idle", false);
        anim.SetBool("Walking", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("Hurt", true);


    }

    public void Die()
    {
        rigidBody.simulated = false;
        box.enabled = false;
        anim.enabled = false;

        for(int i = 0; i < skeletonParts.Length; i++)
        {
            Transform trans = skeletonPartTransform[i];
            trans.rotation *= gameObject.transform.rotation;
            GameObject bone = Instantiate(skeletonParts[i],  trans.position, trans.rotation);
            LimbScript limb = bone.GetComponent<LimbScript>();
            limb.Launch(facingRight);
        }
        skeletonDestroy.SkeletonDest();        
    }

    void AdjustSides()
    {
        if (!attacking)
        {
            if (facingRight)
            {
                gameObject.transform.rotation = rightSide;
                wallBounce.localPosition = new Vector2(0.6f, 1f);
                hitBox.offset = new Vector2(3.629479f, 1.23722f);
            }

            else
            {
                gameObject.transform.rotation = leftSide;
                wallBounce.localPosition = new Vector2(-0.6f, 1f);
                hitBox.offset = new Vector2(-3.629479f, 1.23722f);
            }
        }
    }

    public void SkeletonStopMoving()
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthScript = GameObject.FindGameObjectWithTag("HealthScript").GetComponent<HealthScript>();
        rigidBody = GetComponentInParent<Rigidbody2D>();
        box = GetComponentInParent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        skeletonRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        skeletonDestroy = GetComponentInParent<SkeletonDestroy>();
        rightSide.eulerAngles = new Vector3(0, 120, 0);
        leftSide.eulerAngles = new Vector3(0, 240, 0);
        if (player.position.x <= gameObject.GetComponentInParent<Transform>().position.x)
        {

            facingRight = false;

        }
        else
        {
            facingRight = true;
        }
        AdjustSides();
        preFabTransform = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update () {

        if (invincibleTimer > 0)
        {
            invincibleTimer = invincibleTimer - Time.deltaTime;

        }

        else
        {
            
            foreach (SkinnedMeshRenderer skin in skeletonRenderer)
            {
                skin.material.color = Color.white;
            }
        }

        bool groundhit = Physics2D.OverlapBox(wallBounce.position, new Vector2(0.3f, 0.9f), 0, surface);

        bool collidedSurface = false;

        Collider2D[] collided = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, surface);

        for (int i = 0; i < collided.Length; i++)
        {
            if (collided[i].tag == "Surface")
            {
                collidedSurface = true;
            }
        }

        anim.SetBool("Grounded", collidedSurface);

        if (groundhit)
        {
            facingRight = !facingRight;
            AdjustSides();
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.GetBool("Idle"))
        {
            Idle();
        }

        if (healthScript.GetPlayerHealth() != 0 && !anim.GetBool("Walking")
        && anim.GetBool("Idle")
        && !anim.GetBool("Attacking")
        && !anim.GetBool("Hurt")
        && !anim.GetAnimatorTransitionInfo(0).IsName("Hurt -> Idle"))
        {
            Move();
        }
    }

    void FixedUpdate()
    {

        if(anim.GetAnimatorTransitionInfo(0).IsName("Shoved -> Hurt"))
        {
            anim.SetBool("Hurt", false);
        }

        lastKnownPosition = gameObject.transform.position;
        if (shoved)
        {
            attacking = false;
            rigidBody.velocity = new Vector2(0, 0);

            if (player.position.x <= gameObject.GetComponentInParent<Transform>().position.x)
            {
                facingRight = false;
                AdjustSides();
                rigidBody.velocity = new Vector2(10, 7);

            }
            else
            {
                facingRight = true;
                AdjustSides();
                rigidBody.velocity = new Vector2(-10, 7);
            }

            shoved = false;
        }

        else
        {

            if(anim.GetAnimatorTransitionInfo(0).IsName("Attacking -> Idle"))
            {
                attacking = false;
                anim.SetBool("Attacking", false);
            }

            if (attacking && !anim.GetBool("Hurt"))
            {
                SkeletonStopMoving();
                attacking = false;
            }

            if (anim.GetBool("Grounded")
                && (anim.GetBool("Hurt")
                || anim.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
                && !anim.GetAnimatorTransitionInfo(0).IsName("Shoved -> Hurt")
                && !anim.GetAnimatorTransitionInfo(0).IsName("AnyState -> Shoved")
                && !anim.GetCurrentAnimatorStateInfo(0).IsName("Shoved"))
            {
                SkeletonStopMoving();
                attacking = false;
            }

            if (anim.GetBool("Walking") && anim.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
            {
                Vector2 movement = Vector2.zero;
                attackHit = false;
                if (facingRight)
                {
                    movement = new Vector2(1, rigidBody.velocity.y);
                }
                else
                {
                    movement = new Vector2(-1, rigidBody.velocity.y);
                }

                rigidBody.velocity = movement * speed;
            }

        }
    }

    public void FireArrows()
    {
        Quaternion arrowRotation = new Quaternion();
        if (facingRight)
            arrowRotation.eulerAngles = new Vector3(0, 0, 0);
        else
            arrowRotation.eulerAngles = new Vector3(0, 180, 0);

        GameObject arrowPre = Instantiate(arrow, new Vector3(arrowSpawn.position.x, arrowSpawn.position.y - 1.5f, -5), arrowRotation);
        ProjectileScript projectileScript = arrowPre.GetComponent<ProjectileScript>();
        projectileScript.setDirection(!facingRight);
    }

    

}
