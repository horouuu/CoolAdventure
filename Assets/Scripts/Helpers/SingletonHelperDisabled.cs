using UnityEngine;
public class SingletonHelper : Singleton<PauseScript>
{
    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }
}