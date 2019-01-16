using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour, IMonster {

    int health = 2;
    int points = 0;
    float speed = 1;
    float invincibleTimer = 0;
    private HealthScript healthScript;
    private Animator anim;
    private Rigidbody2D rigidBody;
    private PlayerController pController;
    private Transform player;
    private BoxCollider2D box;
    private bool attacking = false;
    private bool deadBool;
    private ZombieDestroy zombieDestroy;
    private Quaternion rightSide;
    private Quaternion leftSide;
    private Vector3 lastKnownPosition;
    private SkinnedMeshRenderer[] zombieRenderer;
    private Material zombieMaterial;
    [SerializeField]
    private GameObject graveStone;

    public bool shoved;
    public Color hitColor;
    public GameObject hitSpark;
    public BoxCollider2D hitBox;
    public Transform transAttack;
    public Transform wallBounce;
    public Transform groundCheck;
    public LayerMask surface;
    public bool hitTarget;
    public bool attackHit = false;
    public bool facingRight = true;
    [SerializeField]
    public RaycastHit2D[] gameObjectList;


    //Interface
    public void DamageHealth(){health--;}
    public float GetSpeed(){return speed;}
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
        if (!anim.GetBool("Attacking"))
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
                
                anim.SetTrigger("Hurt");
                anim.SetBool("Hurting", true);
                foreach (SkinnedMeshRenderer skin in zombieRenderer)
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
            anim.SetBool("Hurting", true);
           

    }

    public void Die()
    {
        anim.SetTrigger("Dead");
        anim.Play("Dead");
        rigidBody.simulated = false;
        box.enabled = false;
    }



    // Use this for initialization
    void Start () {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthScript = GameObject.FindGameObjectWithTag("HealthScript").GetComponent<HealthScript>();
        rigidBody = GetComponentInParent<Rigidbody2D>();
        box = GetComponentInParent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        zombieRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        zombieDestroy = GetComponentInParent<ZombieDestroy>();
        rightSide.eulerAngles = new Vector3(0, 120, 0);
        leftSide.eulerAngles = new Vector3(0, 240, 0);
        if(player.position.x <= gameObject.GetComponentInParent<Transform>().position.x)
        {

            facingRight = false;

        }
        else
        {
            facingRight = true;
        }
        AdjustSides();
    }
	
	// Update is called once per frame
	void Update () {

        if(graveStone != null)
        {
            float zombieX, zombieY;

            zombieX = player.position.x - gameObject.transform.position.x;
            zombieY = player.position.y - gameObject.transform.position.y;

            //Debug.Log(Mathf.Abs(zombieX) + " ," + Mathf.Abs(zombieY));

            if (Mathf.Abs(zombieX) >= 40 || Mathf.Abs(zombieY) >= 40)
            {
                zombieDestroy.ZombieDest();
            }
        }

        if (invincibleTimer > 0)
        {
            invincibleTimer = invincibleTimer - Time.deltaTime;

        }

        else
        {
            foreach (SkinnedMeshRenderer skin in zombieRenderer)
            {
                skin.material.color = Color.white;
            }
        }

        bool groundhit = Physics2D.OverlapBox(wallBounce.position + new Vector3(0,0.1f,0), new Vector2(0.1f, 1.9f), 0, surface);

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
        && !anim.GetBool("Hurting")
        && !anim.GetAnimatorTransitionInfo(0).IsName("Hurting -> Idle"))
        {
            Move();
        }


        Debug.DrawRay(transAttack.transform.position, Vector3.right);
    }

    void FixedUpdate()
    {
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

            if (attacking && !anim.GetBool("Hurting"))
            {
                ZombieStopMoving();
                attacking = false;
            }

            if (anim.GetBool("Grounded")
                && (anim.GetBool("Hurting")
                || anim.GetCurrentAnimatorStateInfo(0).IsName("Hurting"))
                && !anim.GetAnimatorTransitionInfo(0).IsName("InPainShove -> Hurting")
                && !anim.GetAnimatorTransitionInfo(0).IsName("AnyState -> InPainShove")
                && !anim.GetCurrentAnimatorStateInfo(0).IsName("InPainShove"))
            {
                ZombieStopMoving();

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

            if (deadBool)
            {
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(gameObject.transform.localScale.x, 0f, gameObject.transform.localScale.z), 3f * Time.deltaTime);
                if (gameObject.transform.localScale.y <= 0.01)
                {
                    zombieDestroy.ZombieDest();
                }
            }
        } 
    }


    

    public void ZombieAttack()
    {
        gameObjectList = Physics2D.BoxCastAll(transAttack.transform.position, new Vector2(2, 2), 0, Vector2.zero, 0);

        if (gameObjectList != null) {
            for (int i = 0; i < gameObjectList.Length; i++)
            {
                if(gameObjectList[i].collider.tag == "PGBodyCollider")
                {
                    pController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
                    pController.Hurt(facingRight);
                }
            }
        }
    }

    public void MakeIdle()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Walking", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("Hurting", false);
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
    }

    public void TurnOnDeath()
    {
        deadBool = true;
    }

    public void SetHitBox(BoxCollider2D _box)
    {
        hitBox = _box;
    }

    public void ZombieStopMoving()
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
    }

    void AdjustSides()
    {
        if (facingRight)
        {
            gameObject.transform.rotation = rightSide;
            transAttack.localPosition = new Vector2(0.5f, 1.1f);
            wallBounce.localPosition = new Vector2(0.8f, 1f);
            hitBox.offset = new Vector2(0.9009246f, 1.137205f);
        }

        else
        {
            gameObject.transform.rotation = leftSide;
            transAttack.localPosition = new Vector2(-0.5f, 1.1f);
            wallBounce.localPosition = new Vector2(-0.8f, 1f);
            hitBox.offset = new Vector2(-0.9009246f, 1.137205f);
        }
    }

    public Vector3 ReturnPosition()
    {
        return lastKnownPosition;
    }

    public bool IsZombieHurting()
    {
        return anim.GetBool("Hurting");
    }

    public void ZombieStopAttacking()
    {
        anim.SetBool("Attacking", false);
    }

    public void AssignGraveStone(GameObject grave)
    {
        graveStone = grave;
    }
}
