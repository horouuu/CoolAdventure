using System.Collections;
using UnityEditor.Rendering.Universal;
using UnityEngine;
public class PlayerCombat : MonoBehaviour
{
    // hp
    private bool iFrame = false;

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

    // managers
    public GameManager gameManager;

    // audio source
    public AudioSource gotHitSource;

    public void Attack()
    {
        if (Time.time >= nextAttack)
        {
            animator.SetTrigger("attack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().Damage(1);
            }

            nextAttack = Time.time + 1f / attackDelay;
        }
    }

    public void Revive()
    {
        animator.SetBool("isDead", false);
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
        this.enabled = true;
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
        gotHitSource.PlayOneShot(gotHitSource.clip);

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
        PlayerHealth.SetCurrentHp(PlayerHealth.maxHp);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<Renderer>();
        color = renderer.material.color;
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
}
