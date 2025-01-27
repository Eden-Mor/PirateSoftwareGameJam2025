using System;
using UnityEngine;

public enum CostMultiplier
{
    None = 0,
    Additive,
    TwoTimes,
    Exponential
}

public enum UpgradeFailReason
{
    None,
    MaxCountReached,
    NotEnoughMoney
}

[Serializable]
public class CarUpgradeDetails
{
    public CarUpgradesEnum carUpgradeType;
    public CostMultiplier costMultiplier;
    public int price;
    public int maxCount;
    public string name;
    public string description;
    public bool isEnabled;

    public int GetUpgradePrice(int purchasedUpgradeCount)
        => costMultiplier switch
        {
            CostMultiplier.None => price,
            CostMultiplier.Additive => (purchasedUpgradeCount + 1) * price,
            CostMultiplier.TwoTimes => price * (int)Mathf.Pow(2, purchasedUpgradeCount),
            CostMultiplier.Exponential => price * (int)Mathf.Pow(price, purchasedUpgradeCount),
            _ => price,
        };
}