using UnityEngine;

[CreateAssetMenu(menuName = "CustomUI/ViewSO", fileName = "ViewSO")]
public class ViewSO : ScriptableObject
{
    public ThemeSO theme;
    public RectOffset padding;
    public float spacing;
}
