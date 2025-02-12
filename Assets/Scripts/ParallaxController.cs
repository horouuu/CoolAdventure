using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    float startPosition;
    public GameObject camera;
    public float parallaxSpeed;


    void Start()
    {
        startPosition = transform.position.x;
    }

    void Update()
    {
        float distance = camera.transform.position.x * parallaxSpeed;
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);
    }
}
