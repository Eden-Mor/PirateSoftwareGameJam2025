using UnityEngine;

[CreateAssetMenu(menuName = "CustomUI/ThemeSO", fileName = "Theme")]
public class ThemeSO : ScriptableObject
{
    [Header("Primary")] 
    public Color primary_bg; 
    public Color primary_text;

    [Header("Secondary")] 
    public Color secondary_bg; 
    public Color secondary_text;

    [Header("Button")]
    public Color button_bg;
    public Color button_text;

    [Header("Tertiary")] 
    public Color tertiary_bg; 
    public Color tertiary_text;

    [Header("Other")] 
    public Color disable;

    public Color GetBackgroundColor(Style style) => style switch
    {
        Style.Primary => primary_bg,
        Style.Secondary => secondary_bg,
        Style.Button => button_bg,
        Style.Tertiary => tertiary_bg,
        _ => disable,
    };

    public Color GetTextColor(Style style) => style switch
    {
        Style.Primary => primary_text,
        Style.Secondary => secondary_text,
        Style.Button => button_text,
        Style.Tertiary => tertiary_text,
        _ => disable,
    };
}
