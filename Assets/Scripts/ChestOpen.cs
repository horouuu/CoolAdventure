using NUnit.Framework;
using UnityEngine;

public class ChestOpen : MonoBehaviour, IInteractable
{
    string chestId;
    bool isOpened;
    private Animator animator;
    public GameObject dropItem;

    void Start()
    {
        chestId ??= GlobalHelper.GenerateUUID(gameObject);
        animator = GetComponent<Animator>();
        isOpened = false;
    }
    public void Interact()
    {
        animator.SetBool("opened", true);
        Instantiate(dropItem, transform.position, Quaternion.identity);
        isOpened = true;
    }

    public bool CanInteract()
    {
        return !isOpened;
    }
}
