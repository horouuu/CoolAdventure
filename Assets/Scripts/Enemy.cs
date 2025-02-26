using System.Collections;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHp = 5;
    public int currentHp;
    public Animator animator;
    private float stunDelay = 3f;
    private float stunNext = 0f;
    public GameObject dropItem;
    public ScoreManager scoreManager;
    public AudioSource gotHitSource;

    IEnumerator MiniStun()
    {
        GetComponent<MushroomMovement>().Stun();
        yield return new WaitForSeconds(1);
        GetComponent<MushroomMovement>().Stun();
    }

    public void Damage(int damage)
    {
        currentHp -= damage;
        scoreManager.IncreaseScore(1);

        // hurt anim
        animator.SetTrigger("hit");
        gotHitSource.PlayOneShot(gotHitSource.clip);

        if (currentHp <= 0)
        {
            Die(5);
        }

        // stun if not dead/stunned
        if (!GetComponent<MushroomMovement>().stun && Time.time > stunNext)
        {
            StartCoroutine(MiniStun());
            stunNext = Time.time + stunDelay;
        }
    }

    public void Die(int reward)
    {
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<MushroomMovement>().enabled = false;

        scoreManager.IncreaseScore(reward);
        this.enabled = false;
    }

    void Start()
    {
        currentHp = maxHp;
        animator = GetComponent<Animator>();
        scoreManager = ScoreManager.instance;
    }

    public void DropItem()
    {
        Instantiate(dropItem, transform.position, quaternion.identity);
    }
}
