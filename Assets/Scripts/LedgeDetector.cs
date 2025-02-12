using Unity.VisualScripting;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask groundLayer;

    private bool blockedByWall;


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!(player.GetComponent<PlayerMovement>().isWallGripping || player.GetComponent<PlayerMovement>().isWallSliding))
            {
                blockedByWall = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            blockedByWall = false;
        }
    }

    private void Update()
    {
        if (!blockedByWall)
        {
            player.GetComponent<PlayerMovement>().ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, groundLayer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
