using UnityEngine;

public static class TransformExtensions
{
    public static Transform GetChildWithTag(this Transform parent, string tag)
    {
        foreach (Transform child in parent)
            if (child.CompareTag(tag))
                return child;
        return null;
    }
}
