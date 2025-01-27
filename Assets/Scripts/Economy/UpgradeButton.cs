using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI upgradePrice;

    public CarUpgradesEnum carUpgradeType;

    public void Initialize(CarUpgradeDetails details, UpgradeFailReason failReason, int price)
    {
        carUpgradeType = details.carUpgradeType;
        upgradeName.text = details.name;
        upgradeDescription.text = details.description;
        upgradePrice.text = $"${price}";

        UpdateFailReason(failReason);
    }

    public void UpdatePrice(int price) 
        => upgradePrice.text = $"${price}";

    public void UpdateFailReason(UpgradeFailReason failReason)
    {
        var color = failReason switch
        {
            UpgradeFailReason.None => Color.white,
            UpgradeFailReason.MaxCountReached => Color.gray,
            UpgradeFailReason.NotEnoughMoney => Color.red,
            _ => Color.white,
        };

        ChangeColor(color);
    }

    private void ChangeColor(Color color)
    {
        upgradeIcon.color = color;
        upgradeName.color = color;
        upgradeDescription.color = color;
        upgradePrice.color = color;
    }
}
