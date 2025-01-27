using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Economy/UpgradePricesSO", fileName = "UpgradePricesSO")]
public class UpgradePricesSO : ScriptableObject
{
    public List<CarUpgradeDetails> carUpgrades;

    public int GetUpgradePrice(CarUpgradesEnum upgrade, int purchasedUpgradeCount) 
        => carUpgrades.First(x => x.carUpgradeType == upgrade)
                      .GetUpgradePrice(purchasedUpgradeCount);

    public int GetUpgradeMaxCount(CarUpgradesEnum upgrade) 
        => carUpgrades.First(x => x.carUpgradeType == upgrade).maxCount;
}
