using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombat : MonoBehaviour
{
    // input
    private InputMap playerControls;
    private InputAction attack;

    // hp
    private int maxHp = 3;
    private bool iFrame = false;

    // pts
    private static int points = 0;

    // attack delay
    float attackDelay = 2f;
    float nextAttack = 0f;

    // animator + renderer
    private Animator animator;
    private Renderer renderer;
    Color color;

    // detect enemies
    public LayerMask enemyLayers;
    public float attackRange = 0.5f;
    public Transform attackPoint;

    // game manager
    public GameManagerScript gameManager;

    void Attack()
    {
        animator.SetTrigger("attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().Damage(1);
        }
    }

    public static void Reward(int p)
    {
        ScoreManager.score += p;
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        this.enabled = false;

        gameManager.GameOver();
    }

    IEnumerator IFrame()
    {
        iFrame = true;
        color.a = 0.5f;
        renderer.material.color = color;
        yield return new WaitForSeconds(2);
        iFrame = false;
        color.a = 1f;
        renderer.material.color = color;
    }

    public void Damage(int damage)
    {
        PlayerHealth.SetCurrentHp(PlayerHealth.playerCurrentHp - damage);

        // hurt anim
        animator.SetTrigger("hit");

        if (PlayerHealth.playerCurrentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(IFrame());
        }
    }

    void Awake()
    {
        playerControls = new InputMap();
        PlayerHealth.SetCurrentHp(maxHp);
        points = 0;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<Renderer>();
        color = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttack)
        {
            if (attack.triggered)
            {
                Attack();
                nextAttack = Time.time + 1f / attackDelay;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D entity)
    {
        if (entity.gameObject.layer == 7 && !iFrame)
        {
            Damage(1);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnEnable()
    {
        attack = playerControls.PlayerInputMap.Attack;
        attack.Enable();
    }

    void OnDisable()
    {
        attack.Disable();
    }
}
