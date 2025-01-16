using TMPro;

public class Text : CustomUIComponent
{
    public TextSO textData;
    public Style style;

    private TextMeshProUGUI textMeshProUGUI;

    public override void Setup()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void Configure()
    {
        textMeshProUGUI.color = textData.theme.GetTextColor(style);
        textMeshProUGUI.font = textData.font;
        textMeshProUGUI.fontSize = textData.size;
    }
}
