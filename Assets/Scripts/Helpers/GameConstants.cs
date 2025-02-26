using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "Scriptable Objects/GameConstants")]
public class GameConstants : ScriptableObject
{
    public int maxHp = 3;
    public Vector3 startingPosition;
}
