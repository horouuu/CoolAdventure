using UnityEngine;

public class ClimbPower : MonoBehaviour
{
    public AudioSource powerupAudio;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            col.GetComponent<PlayerMovement>().climbPower = true;
            powerupAudio.PlayOneShot(powerupAudio.clip);
            GetComponent<SpriteRenderer>().enabled = false;
            Invoke(nameof(Deactivate), 1f);
        }
    }

    private void Deactivate() => gameObject.SetActive(false);
}
