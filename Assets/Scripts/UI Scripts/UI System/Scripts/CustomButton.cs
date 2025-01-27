using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomButton : CustomUIComponent
{
    public ThemeSO theme;
    public Style style;
    public UnityEvent onClick;

    private Button button;
    private TextMeshProUGUI buttonText;
    private Image buttonIcon;

    public override void Setup()
    {
        button = GetComponentInChildren<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonIcon = GetComponentInChildren<Image>();
    }

    public override void Configure()
    {
        ColorBlock cb = button.colors;
        cb.highlightedColor = theme.GetBackgroundColor(style);
        button.colors = cb;

        if (buttonText != null)
            buttonText.color = theme.GetTextColor(style);
        else if (buttonIcon != null)
            buttonIcon.color = theme.GetTextColor(style);
    }

    public void OnClick() => onClick.Invoke();
}
