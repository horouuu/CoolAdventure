using Unity.VisualScripting;
using UnityEngine;

public class GameOverUICanvas : Singleton<GameOverUICanvas>
{
    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        transform.gameObject.SetActive(false);
    }
}
