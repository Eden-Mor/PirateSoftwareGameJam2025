using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CarUpgradeDisplay : MonoBehaviour
{
    [SerializeField] private UpgradePricesSO upgradePriceSO;
    [SerializeField] private UpgradeButton upgradePrefab;
    [SerializeField] private EconomyManager economyManager;
    private List<UpgradeButton> buttonList = new();
    
    private void OnEnable()
    {
        if (this.transform.childCount == 0)
        {
            foreach (var upgrade in upgradePriceSO.carUpgrades)
            {
                if (!upgrade.isEnabled)
                    continue;

                var upgradeObj = Instantiate(upgradePrefab, this.transform);
                
                var button = upgradeObj.GetComponent<Button>();
                button.onClick.AddListener(() => economyManager.BuyUpgrade(upgrade.carUpgradeType));

                var purchasedUpgradeCount = economyManager.GetPurchasedUpgradeCount(upgrade.carUpgradeType);
                var price = upgrade.GetUpgradePrice(purchasedUpgradeCount);
                upgradeObj.Initialize(upgrade, 
                                      economyManager.CanBuyUpgrade(upgrade.carUpgradeType),
                                      price);

                buttonList.Add(upgradeObj);
            }
        }
        else
        {
            UpdateExistingRecords();
        }

        EventManager.Economy.OnPurchaseMade.Get().AddListener(UpdateExistingRecords);
    }

    private void OnDisable()
    {
        EventManager.Economy.OnPurchaseMade.Get().RemoveListener(UpdateExistingRecords);
    }

    private void UpdateExistingRecords()
    {
        foreach (UpgradeButton child in buttonList)
        {
            child.UpdateFailReason(economyManager.CanBuyUpgrade(child.carUpgradeType));

            var purchasedUpgradeCount = economyManager.GetPurchasedUpgradeCount(child.carUpgradeType);
            var price = upgradePriceSO.GetUpgradePrice(child.carUpgradeType, purchasedUpgradeCount);
            child.UpdatePrice(price);
        }
    }
}
