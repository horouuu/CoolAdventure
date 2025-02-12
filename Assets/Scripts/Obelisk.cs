using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Obelisk : MonoBehaviour, IInteractable
{
    public bool isActive { get; private set; }
    public string obeliskId { get; private set; }
    public GameObject sigil;
    public AudioSource sigilAudio;
    public AudioSource teleportAudio;
    public GameObject player;
    public GameObject camera;
    public GameObject linkedObelisk;
    private SpriteRenderer sigilSprite;
    void Start()
    {
        obeliskId ??= GlobalHelper.GenerateUUID(gameObject);
        sigilSprite = sigil.GetComponent<SpriteRenderer>();
        isActive = false;
    }

    public bool CanInteract()
    {
        return (linkedObelisk.GetComponent<Obelisk>().isActive && isActive) || !isActive;
    }

    public void Interact()
    {
        if (CanInteract())
        {
            if (!isActive)
            {
                SetActive(true);
            }
            else if (linkedObelisk.GetComponent<Obelisk>().isActive)
            {
                StartCoroutine(TeleportPlayer());
            }
        }
    }

    public void SetActive(bool active)
    {
        if (isActive = active)
        {
            sigil.SetActive(true);
            StartCoroutine(FadeInSigil());
        }
    }

    private IEnumerator TeleportPlayer()
    {
        CinemachinePositionComposer positionComposer = camera.GetComponent<CinemachinePositionComposer>();
        Vector3 originalDamping = new Vector3(positionComposer.Damping.x, positionComposer.Damping.y, positionComposer.Damping.z);
        positionComposer.Damping.Set(0, 0, 0);
        teleportAudio.PlayOneShot(teleportAudio.clip);
        player.transform.SetPositionAndRotation(linkedObelisk.transform.position + new Vector3(0, 0.2f, 0), player.transform.rotation);
        yield return new WaitForSeconds(0.1f);
        positionComposer.Damping.Set(originalDamping.x, originalDamping.y, originalDamping.z);
    }

    private IEnumerator FadeInSigil()
    {
        sigilAudio.PlayOneShot(sigilAudio.clip);
        float alphaVal = sigilSprite.color.a;
        Color tmp = sigilSprite.color;

        while (sigilSprite.color.a < 1)
        {
            alphaVal += 0.01f;
            tmp.a = alphaVal;
            sigilSprite.color = tmp;

            yield return new WaitForSeconds(0.01f);
        }
    }
}
