using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D col;   
    public AudioSource sfx;
    public AudioClip damageSound;
    private enum Character {Player, Enemy};
    private Character currentCharacter;

    [SerializeField] private int maxHealth;
    private int health;

    private enum Side{LEFT, RIGHT};
    [SerializeField] private Vector2 damageForceVector = new Vector2(100, 100);

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        col = gameObject.GetComponent<BoxCollider2D>();

        health = maxHealth;

        if (gameObject.tag == "Player")
        {
            currentCharacter = Character.Player;
        }
        else
        {
            currentCharacter = Character.Enemy;
            Debug.Log("Statement 0");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }  

    void OnCollisionEnter2D(Collision2D col)
    {
        if (currentCharacter == Character.Player && col.gameObject.tag == "Enemy")
        {
            if (col.transform.position.x >= transform.position.x)
            {
                Damage(Side.RIGHT);
            }
            else
            {
                Damage(Side.LEFT);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (currentCharacter == Character.Enemy &&
        (col.gameObject.name == "AttackHit"
        || col.gameObject.name == "CrouchHit"
        || col.gameObject.name == "AttackUpHit"
        || col.gameObject.name == "AttackDownHit")
        && !anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
        {
            Debug.Log("Statement 1");
            if (col.transform.position.x >= transform.position.x)
            {
                Damage(Side.RIGHT);
            }
            else
            {
                Damage(Side.LEFT);
            }
        }
    }

    void Damage(Side enemySide)
    {
        if (enemySide == Side.LEFT)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            damageForceVector.x = Mathf.Abs(damageForceVector.x);
            rb.AddForce(damageForceVector);
            anim.Play("Damaged");
        }

        if (enemySide == Side.RIGHT)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            damageForceVector.x = -Mathf.Abs(damageForceVector.x);
            rb.AddForce(damageForceVector);
            anim.Play("Damaged");
        }
        health -= 1;
        if (ShouldDie())
        {
            if (currentCharacter == Character.Player)
                PlayerDie();
            else
                EnemyDie();
        }
    }

    bool ShouldDie()
    {
        return health == 0;
    }

    void PlayerDie()
    {
        SceneManager.LoadScene(1);
    }

    void EnemyDie()
    {
        Destroy(gameObject);
    }

}
