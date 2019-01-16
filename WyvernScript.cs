using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernScript : MonoBehaviour, IMonster
{

    int points = 0;
    float xVelocity;
    float yVelocity;
    float invincibleTimer = 0;
    private HealthScript healthScript;
    private Animator anim;
    private Rigidbody2D wyvernRigidBody;
    private PlayerController pController;
    private Transform player;
    private BoxCollider2D box;
    private ZombieDestroy wyvernDestroy;
    private bool attacking = false;
    private bool deadBool;
    private Quaternion rightSide;
    private Quaternion leftSide;
    private Quaternion rotationUpLeft;
    private Quaternion rotationUpRight;
    private Vector3 lastKnownPosition;
    private SkinnedMeshRenderer[] wyvernRenderer;
    private Material wyvernMaterial;
    private Transform preFabTransform;
    private Vector2 velocity;
    private Vector2 positionToFly;
    [SerializeField]
    private float shovedVelocity = 25;
    private float swoopTime;
    private float swoopRadius = -15f;
    private float swoopAngle = 0;
    private float swoopAngleTime = 0.25f;
    private float swoopAngleCount = 12;
    private float swoopDecrease = 1;
    private float swoopDecreaseMultiplier = 1;
    private float deathTimer = 3;
    private bool rotateUp;
    private bool rotateBack;
    [SerializeField]
    private bool isShoved;
    private bool isActivated;

    public bool isWaiting;
    public bool isChild = true;
    public float maxSpeed;
    public float deceleration;
    public float timeZeroToMax;
    public float acceleration;
    public int health;
    public Color hitColor;
    public GameObject hitSpark;
    public BoxCollider2D hitBox;
    public bool attackHit = false;
    public bool facingRight = true;
    public bool readyToAttack;

    public void DamageHealth() { health--; }
    public float GetSpeed() { return maxSpeed; }
    public int ReturnPoints() { return points; }

    public void Idle()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Walking", false);
    }

    public void Attack()
    {

        if (isChild)
        {
            
            swoopDecrease += 0.05f * 1 + Time.deltaTime * swoopDecreaseMultiplier;
            swoopDecreaseMultiplier += 1f * Time.deltaTime * 2;

            Vector2 swoopVelocity = new Vector2(swoopRadius / swoopDecrease * Mathf.Cos(swoopAngle), swoopRadius / swoopDecrease * Mathf.Sin(swoopAngle));

            wyvernRigidBody.velocity = swoopVelocity;
            swoopTime = swoopTime - Time.deltaTime;
            

            if (facingRight)
            {
                swoopAngle += swoopAngleCount * Mathf.Deg2Rad * swoopAngleTime;
            }
            else
            {
                swoopAngle -= swoopAngleCount * Mathf.Deg2Rad * swoopAngleTime;
            }

            swoopAngleTime -= 0.005f;
            swoopAngleCount += 0.02f;

            if (swoopAngleTime < 0)
            {
                swoopAngleTime = 0;
            }

            if (swoopTime <= 0)
            {
                attacking = false;
                readyToAttack = false;
                swoopTime = 2.5f;
                swoopAngleTime = 0.25f;
                swoopAngleCount = 12;
                swoopDecrease = 1;
                wyvernRigidBody.velocity = Vector2.zero;
                AdjustSides();
                swoopDecreaseMultiplier = 1;
            }
        }

    }

    public void Move()
    {

        wyvernRigidBody.velocity = new Vector2(xVelocity, yVelocity);

    }

    public void TakeDamage()
    {
        if (invincibleTimer <= 0)
        {
            points -= 1;
            Instantiate(hitSpark, hitBox.transform.position, Quaternion.identity);

            if (points <= 0)
            {
                Die();
            }
        }

    }

    public void GetShoved()
    {
        if (isShoved == false)
        {
            isShoved = true;
        }
    }

    public void Die()
    {
        rotateUp = false;
        rotateBack = false;
      
        if (facingRight)
        {
            gameObject.transform.rotation = Quaternion.Euler(0,rightSide.y,0);
        }

        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, leftSide.y, 0);
        }

        box.enabled = false;
        deadBool = true;
        wyvernRigidBody.velocity = Vector2.zero;
        wyvernRigidBody.simulated = true;
        wyvernRigidBody.gravityScale = 3;
        

        anim.SetTrigger("Dead");
        float randomNum = Random.Range(-4, 4);
        wyvernRigidBody.AddForce(new Vector2(randomNum * 50, 600));
    }

    void AdjustSides()
    {
        {
            if (player.position.x < preFabTransform.position.x)
                facingRight = false;

            else
                facingRight = true;

            if (facingRight)
            {
                swoopAngle = 90;
            }
            else
            {
                swoopAngle = 45;
            }

        }
    }

    void Start()
    {


        isActivated = false;
        isShoved = false;
        swoopTime = 2.5f;
        readyToAttack = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthScript = GameObject.FindGameObjectWithTag("HealthScript").GetComponent<HealthScript>();
        wyvernRigidBody = GetComponentInParent<Rigidbody2D>();
        box = GetComponentInParent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        wyvernRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        wyvernDestroy = GetComponentInParent<ZombieDestroy>();
        rightSide.eulerAngles = new Vector3(0, 120, 0);
        leftSide.eulerAngles = new Vector3(0, 240, 0);
        rotationUpLeft.eulerAngles = new Vector3(-50, 240, 0);
        rotationUpRight.eulerAngles = new Vector3(-50, 120, 0);
        preFabTransform = GetComponentInParent<Transform>();
        AdjustSides();
        if (isChild)
            points = 1;
        else
            points = 2;

        if (facingRight)
            gameObject.transform.rotation = rightSide;
        else
            gameObject.transform.rotation = leftSide;
    }

    void Update()
    {
        if (!isActivated)
        {
            float wyvernX, wyvernY;

            wyvernX = player.position.x - preFabTransform.position.x;
            wyvernY = player.position.y - preFabTransform.position.y;

            //Debug.Log(Mathf.Abs(wyvernX) + " ," + Mathf.Abs(wyvernY));

            if (Mathf.Abs(wyvernX) <= 10 && Mathf.Abs(wyvernY) <= 10)
            {
                if (!isWaiting)
                {
                    ActivateWyvern();
                }
            }
            
            if (Mathf.Abs(wyvernX) >= 50 && Mathf.Abs(wyvernY) >= 50)
            {
                DeactivateWyvern();
            }
        }
        



        if (deadBool)
        {
            deathTimer -= Time.deltaTime;
            if(deathTimer <= 0)
            {
                wyvernDestroy.ZombieDest();
            }
        }

        if (!readyToAttack || !attacking)
        {
            AdjustSides();
        }


        if (invincibleTimer > 0)
        {
            invincibleTimer = invincibleTimer - Time.deltaTime;

        }

        else
        {

            foreach (SkinnedMeshRenderer skin in wyvernRenderer)
            {
                skin.material.color = Color.white;
            }
        }


    }

    void FixedUpdate()
    {
        if (isActivated)
        {

            if (isShoved)
            {
                shovedVelocity -= 25 * Time.deltaTime;
                attacking = false;
                readyToAttack = false;
                swoopTime = 2.5f;
                swoopAngleTime = 0.25f;
                swoopAngleCount = 12;
                swoopDecrease = 1;
                swoopDecreaseMultiplier = 1;
                if (shovedVelocity <= 0)
                {
                    isShoved = false;
                    shovedVelocity = 25;
                }
            }

            if (rotateUp)
            {
                if (facingRight)
                {
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, rotationUpRight, 6.5f * Time.deltaTime);
                }

                else
                {
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, rotationUpLeft, 6.5f * Time.deltaTime);
                }
            }

            if (rotateBack)
            {
                if (facingRight)
                {
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, rightSide, 5f * Time.deltaTime);
                }

                else
                {
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, leftSide, 5f * Time.deltaTime);
                }
            }


            if (!deadBool)
            {
                if (!attacking)
                {


                    if (preFabTransform.position.x < player.position.x)
                    {
                        positionToFly = new Vector2(player.position.x - 1, player.position.y + 3);
                    }
                    else
                    {
                        positionToFly = new Vector2(player.position.x + 1, player.position.y + 3);
                    }

                }

                else
                {
                    if (!readyToAttack)
                        positionToFly = new Vector2(player.position.x, player.position.y + 0.5f);
                }

                if (!attacking && !isShoved)
                {
                    if (positionToFly.x - preFabTransform.position.x <= 1.5
                        && positionToFly.x - preFabTransform.position.x >= -1.5
                        && positionToFly.y - preFabTransform.position.y <= 1.5
                        && positionToFly.y - preFabTransform.position.y >= -1.5
                        )
                    {
                        attacking = true;
                        anim.SetTrigger("Attacking");
                    }
                }

                float xClamp = Mathf.Clamp(positionToFly.x - preFabTransform.position.x, -1, 1);
                float yClamp = Mathf.Clamp(positionToFly.y - preFabTransform.position.y, -1, 1);

                if (!isShoved)
                {
                    xVelocity += acceleration / timeZeroToMax * xClamp;
                }

                else
                {
                    if (facingRight)
                        xVelocity = -shovedVelocity;
                    else
                        xVelocity = shovedVelocity;
                }

                yVelocity += acceleration / timeZeroToMax * yClamp;

                if (!isShoved)
                {
                    xVelocity = Mathf.Clamp(xVelocity, -maxSpeed, maxSpeed);
                }

                yVelocity = Mathf.Clamp(yVelocity, -maxSpeed, maxSpeed);

                if (facingRight)
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, rightSide, Time.deltaTime * 2);
                else
                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, leftSide, Time.deltaTime * 2);

                if (!attacking)
                {
                    Move();
                }

                else
                {
                    float speedMul = 1;

                    speedMul -= Time.deltaTime * deceleration;

                    wyvernRigidBody.velocity = wyvernRigidBody.velocity * speedMul;

                    if (Mathf.Abs(wyvernRigidBody.velocity.x) <= 0.025 && Mathf.Abs(wyvernRigidBody.velocity.y) <= 0.025)
                    {
                        readyToAttack = true;
                    }
                    if (readyToAttack)
                    {
                        Attack();
                    }


                }
            }
        }

        else
        {
            wyvernRigidBody.velocity = new Vector2(0, 0);
        }
    }

    public void RotateUpSwitch()
    {
        rotateUp = !rotateUp;
    }

    public void RotateBackSwitch()
    {
        rotateBack = !rotateBack;
    }

    public bool IsWyvernShoved()
    {
        return isShoved;
    }

    public void ActivateWyvern()
    {
        isActivated = true;
    }

    public void DeactivateWyvern()
    {
        isActivated = false;
    }
}
