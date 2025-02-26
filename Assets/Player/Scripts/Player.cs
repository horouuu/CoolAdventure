using UnityEngine;
public class Player : Singleton<Player>
{
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerHealth health;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public PlayerInteraction interaction;
    [HideInInspector] public AnimationEventHelper AEHelper;
    [HideInInspector] public Animator animator;
    override public void Awake()
    {
        base.Awake();

        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
        combat = GetComponent<PlayerCombat>();
        interaction = GetComponent<PlayerInteraction>();
        AEHelper = GetComponent<AnimationEventHelper>();
        animator = GetComponent<Animator>();
    }
}
