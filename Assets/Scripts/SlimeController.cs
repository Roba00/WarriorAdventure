using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    [SerializeField] private int totalHealth;
    private int health;
    private enum Side { LEFT, RIGHT };
    [SerializeField] private Vector2 damageForceVector = new Vector2(1000, 10);

    private RaycastHit2D leftGroundHit;
    private RaycastHit2D rightGroundHit;
    [SerializeField] private Vector2 leftRaycastOrigin;
    [SerializeField] private Vector2 rightRaycastOrigin;
    [SerializeField] private float raycastDistance;
    Side moveDirection;

    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        health = totalHealth;
        moveDirection = Side.LEFT;
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlatform();
        if (moveDirection == Side.LEFT)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            if (isGrounded) rb.velocity = new Vector2(-1, rb.velocity.y);
        }
        else
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            if (isGrounded) rb.velocity = new Vector2(1, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "AttackHit"
            || other.gameObject.name == "CrouchHit"
            || other.gameObject.name == "AttackUpHit"
            || other.gameObject.name == "AttackDownHit")
        {
            if (other.transform.position.x >= transform.position.x)
            {
                //Damage(Side.RIGHT);
            }
            else
            {
                //Damage(Side.LEFT);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
            Debug.Log("Ungrounded");
        }
    }

    void Damage(Side playerSide)
    {
        Debug.Log("Enemy attacked!");
        if (playerSide == Side.LEFT)
        {
            damageForceVector.x = Mathf.Abs(damageForceVector.x);
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.AddForce(damageForceVector);
            //anim.Play("Damaged");
        }

        if (playerSide == Side.RIGHT)
        {
            damageForceVector.x = Mathf.Abs(damageForceVector.x); //Used to be posotive
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.AddForce(damageForceVector);
            //anim.Play("Damaged");
        }
    }

    void DetectPlatform()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        leftGroundHit = Physics2D.Raycast(pos+leftRaycastOrigin, Vector2.down, raycastDistance);
        Vector2 endL = new Vector2(leftRaycastOrigin.x, leftRaycastOrigin.y - raycastDistance);
        Debug.DrawLine(pos+leftRaycastOrigin, pos+endL, Color.red);

        rightGroundHit = Physics2D.Raycast(pos+rightRaycastOrigin, Vector2.down, raycastDistance);
        Vector2 endR = new Vector2(rightRaycastOrigin.x, rightRaycastOrigin.y - raycastDistance);
        Debug.DrawLine(pos+rightRaycastOrigin, pos+endR, Color.red);

        if (!leftGroundHit && rightGroundHit && moveDirection == Side.LEFT)
        {
            moveDirection = Side.RIGHT;
        }
        if (!rightGroundHit && leftGroundHit && moveDirection == Side.RIGHT)
        {
            moveDirection = Side.LEFT;
        }
    }
}
