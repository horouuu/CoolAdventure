using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    float startPosition;
    public GameObject camera;
    public float parallaxSpeed;


    void Awake()
    {
        camera = GameObject.Find("Main Camera");
    }

    void Start()
    {
        startPosition = transform.position.x;
    }

    void Update()
    {
        camera = GameObject.Find("Main Camera");
        float distance = camera.transform.position.x * parallaxSpeed;
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);
    }
}
