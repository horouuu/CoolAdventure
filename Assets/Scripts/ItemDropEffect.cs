using UnityEngine;

public class ItemDropEffect : MonoBehaviour
{
    private Rigidbody2D itemBody;
    public float dropForce = 5;
    void Start()
    {
        itemBody = GetComponent<Rigidbody2D>();
        itemBody.AddForce(Vector2.up * dropForce, ForceMode2D.Impulse);
    }
}
