using Unity.Cinemachine;
using UnityEngine;

public class FollowCamera : Singleton<FollowCamera>
{
    CinemachineConfiner2D confiner;
    public override void Awake()
    {
        base.Awake();
        confiner = this.GetComponent<CinemachineConfiner2D>();
    }

    void Update()
    {
        if (GameObject.Find("bound") != null)
            confiner.BoundingShape2D = GameObject.Find("bound").GetComponent<BoxCollider2D>();
    }
}
