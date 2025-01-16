using UnityEngine;
using UnityEngine.UI;

public class View : CustomUIComponent
{
    public ViewSO viewData;

    public GameObject containerTop;
    public GameObject containerCenter;
    public GameObject containerBottom;

    private Image imageTop;
    private Image imageCenter;
    private Image imageBottom;

    private VerticalLayoutGroup verticalLayoutGroup;

    public override void Setup()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        imageTop = containerTop.GetComponent<Image>();
        imageCenter = containerCenter.GetComponent<Image>();
        imageBottom = containerBottom.GetComponent<Image>();
    }

    public override void Configure()
    {
        verticalLayoutGroup.padding = viewData.padding;
        verticalLayoutGroup.spacing = viewData.spacing;

        imageTop.color = viewData.theme.primary_bg;
        imageCenter.color = viewData.theme.secondary_bg;
        imageBottom.color = viewData.theme.tertiary_bg;
    }
}
