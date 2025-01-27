using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FillAmountController : MonoBehaviour
{
    public Color positiveColor = Color.magenta;
    public Color negativeColor = Color.yellow;

    private Image[] childImages;
    
    void Start()
    {
        childImages = GetComponentsInChildren<Image>();
        UpdateFillAmounts(-5f);
    }

    public void UpdateFillAmounts(float value)
    {
        if (childImages == null)
            return;

        for (int i = 0; i < childImages.Length; i++)
        {
            childImages[i].fillAmount = Mathf.Clamp01(Mathf.Abs(value) - i);
            childImages[i].color = value >= 0 ? positiveColor : negativeColor;
        }
    }
}
