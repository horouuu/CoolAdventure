using UnityEngine;

public static class GlobalHelper
{
    public static string GenerateUUID(GameObject obj) {
        return $"{obj.scene.name}-{obj.name}-{obj.transform.position.x}-{obj.transform.position.y}";
    }
}