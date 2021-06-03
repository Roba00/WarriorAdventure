using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlyrController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D col;
    public AudioSource sfx;
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public AudioClip airAttackSound;
    [SerializeField] private float walkForce =10f;
    [SerializeField] private float maxWalkSpeed =2.5f;
    [SerializeField] private float maxAirSpeed =2f;
    [SerializeField] private float jumpSpeed =4f;
    [SerializeField] private float attackUpForce =5f;
    [SerializeField] private float attackDownForce =5f;
    private bool isGrounded;
    private bool hasAirAttackUp;
    private bool hasAirAttackDown;
    private bool jumpButtonReleased;
    private float jumpTimeCounter;
    [SerializeField] private float jumpTime = 0.25f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        col = gameObject.GetComponent<BoxCollider2D>();
        //Time.timeScale = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        Direction();
        GroundedCheck();
        Jump();
        Attack();
        Crouch();
        JumpAttacks();
        Block();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch")
        && !anim.GetCurrentAnimatorStateInfo(0).IsName("CrouchAttack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Block")
        && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged")  && !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackUp")
         && !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackDown"))
            rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal")*walkForce, 0));
            if (isGrounded)
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxWalkSpeed, maxWalkSpeed), rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxAirSpeed, maxAirSpeed), rb.velocity.y);
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void Direction()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackUp") && !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackDown")
        && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            if (Input.GetKey("a"))
            {
                transform.localScale = new Vector3(-1,1,1);
            }

            if (Input.GetKey("d"))
            {
                transform.localScale = new Vector3(1,1,1);
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown("o") && isGrounded)
        {  
            jumpButtonReleased = false;
            jumpTimeCounter = jumpTime;
            anim.Play("PrepJump");
            sfx.clip = jumpSound;
            sfx.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed/3);
            //rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if (Input.GetKeyUp("o"))
        {
            jumpButtonReleased = true;
        }

        if (Input.GetKey("o") && !jumpButtonReleased)
        {  
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                jumpButtonReleased = true;
            }
        }
    }

    void Crouch()
    {
        if (Input.GetKey("s") && (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")))
        {
            anim.Play("Crouch");
        }
        if (Input.GetKeyUp("s") && anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch"))
        {
            anim.Play("Idle");
        }

        if (Input.GetKeyUp("p") && anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch"))
        {
            anim.Play("CrouchAttack");
            sfx.clip = attackSound;
            sfx.Play();
        }
        if (Input.GetKeyUp("s") && anim.GetCurrentAnimatorStateInfo(0).IsName("CrouchAttack"))
        {
            anim.Play("Idle");
        }
    }

    void JumpAttacks()
    {
        if (Input.GetKeyDown("p") && Input.GetKey("w") && (Input.GetKey("d") || Input.GetKey("a")) && !isGrounded && !hasAirAttackUp
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Block")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {  
            jumpButtonReleased = true;
            hasAirAttackUp = true;
            anim.Play("AttackUp");
            sfx.clip = airAttackSound;
            sfx.Play();
            rb.gravityScale = 0;
            //rb.velocity = new Vector2(rb.velocity.x*2, 0);
            rb.velocity = new Vector2(maxWalkSpeed*transform.localScale.x, 0); //New
            rb.AddForce(new Vector2(0, attackUpForce), ForceMode2D.Impulse);
            rb.gravityScale = 1;
        }

        if (Input.GetKeyDown("p") && Input.GetKey("s") && (Input.GetKey("d") || Input.GetKey("a")) && !isGrounded && !hasAirAttackDown
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Block")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {  
            jumpButtonReleased = true;
            hasAirAttackDown = true;
            anim.Play("AttackDown");
            sfx.clip = airAttackSound;
            sfx.Play();
            rb.gravityScale = 0;
            //rb.velocity = new Vector2(rb.velocity.x*2, 0);
            rb.velocity = new Vector2(maxWalkSpeed*transform.localScale.x, 0); //New
            rb.AddForce(new Vector2(0, -attackDownForce), ForceMode2D.Impulse);
            rb.gravityScale = 1;
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown("p") && !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackUp") && 
            !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackDown")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch") 
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("CrouchAttack")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Block")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            if (!(Input.GetKey("w") && (Input.GetKey("d") || Input.GetKey("a")) && !isGrounded && !hasAirAttackUp) ||
            !(Input.GetKey("s") && (Input.GetKey("d") || Input.GetKey("a")) && !isGrounded && !hasAirAttackDown))
            {
                anim.Play("Attack");
                sfx.clip = attackSound;
                sfx.Play();
            }
        }
    }

    void Block()
    {
        if (Input.GetKeyDown("[") && !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackUp") && 
            !anim.GetCurrentAnimatorStateInfo(0).IsName("AttackDown")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch") 
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("CrouchAttack")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Block")
            && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            anim.Play("Block");
        }
    }
    
    void GroundedCheck()
    {
        Vector2 center = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 size = new Vector2(col.size.x-0.125f, 0.25f);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        if (Physics2D.OverlapBox(center, size, 0, layerMask))
        {
            isGrounded = true;
            hasAirAttackUp = false;
            hasAirAttackDown = false;
        }
        else
        {
            isGrounded = false;
        }
        anim.SetBool("Grounded", isGrounded);
    }

    public void RefillJumpAttacks()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        hasAirAttackUp = false;
        hasAirAttackDown = false;
    }
}
