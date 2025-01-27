using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private int startingCash = 30;
    [SerializeField] private int currentCash;
    [SerializeField] private UpgradePricesSO upgradePriceSO;
    [SerializeField] private List<CarUpgradeData> purchasedUpgrades = new();
    [SerializeField] private AudioSource coinEarned;
    [SerializeField] private AudioSource coinSpent;

    private void Start()
    {
        AddCash(startingCash);

        var carUpgrades = Enum.GetValues(typeof(CarUpgradesEnum));
        foreach (CarUpgradesEnum upgradeType in carUpgrades)
            purchasedUpgrades.Add(new CarUpgradeData() { type = upgradeType, count = 0 });

        EventManager.Player.OnReviewFinished.Get().AddListener(AddCashBasedOnReview);
    }

    private void AddCashBasedOnReview(float stars)
    {
        //Convert the range of -5 to 5 
        //to a range of 0 to 10
        //0 corresponding to "5 yellow stars"
        stars += 5f;

        //Add a free star
        stars += 1f;

        AddCash(Mathf.FloorToInt(stars * 5));
    }

    public void AddCash(int amount)
    {
        currentCash += Mathf.Clamp(amount, -currentCash, int.MaxValue);
        EventManager.Game.OnCashChanged.Get().Invoke(currentCash);

        if (amount > 0)
            coinEarned.Play();
        else
            coinSpent.Play();
    }

    public int GetPurchasedUpgradeCount(CarUpgradesEnum upgradeType)
        => GetPurchasedUpgrade(upgradeType).count;

    public CarUpgradeData GetPurchasedUpgrade(CarUpgradesEnum upgradeType) 
        => purchasedUpgrades.First(x => x.type == upgradeType);

    public UpgradeFailReason CanBuyUpgrade(CarUpgradesEnum upgrade)
    {
        var purchasedUpgradeCount = GetPurchasedUpgradeCount(upgrade);

        //If theyre trying to exceed the max count
        if (upgradePriceSO.GetUpgradeMaxCount(upgrade) <= purchasedUpgradeCount)
            return UpgradeFailReason.MaxCountReached;

        //If they dont have enough money
        var price = upgradePriceSO.GetUpgradePrice(upgrade, purchasedUpgradeCount);
        if (price > currentCash)
            return UpgradeFailReason.NotEnoughMoney;

        return UpgradeFailReason.None;
    }

    public void BuyUpgrade(CarUpgradesEnum upgrade)
    {
        if (CanBuyUpgrade(upgrade) != UpgradeFailReason.None)
            return;

        var currentUpgrade = purchasedUpgrades.First(x => x.type == upgrade);
        var price = upgradePriceSO.GetUpgradePrice(upgrade, currentUpgrade.count);

        AddCash(-price);
        currentUpgrade.count++;

        EventManager.Economy.OnPurchaseMade.Get().Invoke();
    }
}
