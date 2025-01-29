using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ReviewManager : MonoBehaviour
{
    public Timer timer;
    public EconomyManager economyManager;
    public FillAmountController fillAmountController;

    [Range(-1f, 0f), SerializeField]
    private float reviewTickDownAmount = -0.1f;

    [Range(0, 10), SerializeField]
    private int reviewTickDownRate = -1;

    [Range(-5f, 5f), SerializeField]
    private float reviewCounter = -5f;

    [SerializeField]
    private bool tickDownReview = true;

    [SerializeField]
    private bool isReviewing = false;

    [SerializeField] private bool isFailed = false;

    private UnityAction reviewStopListener;
    private GameObject fillAmountParent;
    private int tempReviewScore = -5;

    private void Start()
    {
        timer.OnTimerComplete.AddListener(FailReview);
        reviewStopListener = () => StartCoroutine(OnReviewStop());
        EventManager.Player.OnCarCollide.Get().AddListener(OnCarCollided);
        EventManager.Player.OnReviewStop.Get().AddListener(reviewStopListener);
        EventManager.Player.OnReviewStart.Get().AddListener(OnReviewStart);
        EventManager.Player.OnCarHonked.Get().AddListener(OnCarHonked);
        fillAmountParent = fillAmountController.transform.parent.gameObject;

        fillAmountParent.SetActive(false);
    }

    private void FailReview() 
        => isFailed = true;

    public float GetReviewValue()
        => reviewCounter;

    IEnumerator StartReviewTicking(int speakerSystemUpgrade)
    {
        while (isReviewing)
        {
            yield return new WaitForSeconds(reviewTickDownRate);

            var speakerSystemModifier = speakerSystemUpgrade * 0.1f;

            AddToReviewCounter(tickDownReview ? reviewTickDownAmount : 0f + speakerSystemModifier);

            var roundedReviewScore = Mathf.FloorToInt(reviewCounter);

            if(roundedReviewScore > tempReviewScore) //Gained a star
                EventManager.ChatBubble.OnCustomerReaction.Get().Invoke(CustomerInteraction.ReactionType.StarGained);
            else if (roundedReviewScore < tempReviewScore) //Lost a star
                EventManager.ChatBubble.OnCustomerReaction.Get().Invoke(CustomerInteraction.ReactionType.StarLost);

            tempReviewScore = roundedReviewScore;
        }
    }

    private void AddToReviewCounter(float amount)
        => SetReviewCounter(reviewCounter + amount);

    private void SetReviewCounter(float amount)
    {
        reviewCounter = Mathf.Clamp(amount, -5f, 5f);
        fillAmountController.UpdateFillAmounts(reviewCounter);
    }

    private void OnCarCollided(Component component, int velocity)
    {
        // in the future check what component it was colliding with to determine better values for decreasing the review count;
        if (!isReviewing)
            return;

        AddToReviewCounter(velocity / 100f);
        EventManager.ChatBubble.OnCustomerReaction.Get().Invoke(CustomerInteraction.ReactionType.CarCollision);
    }

    private void OnCarHonked()
    {
        if (isReviewing)
            AddToReviewCounter(0.50f);
    }

    private IEnumerator OnReviewStop()
    {
        isReviewing = false;
        tickDownReview = false;

        EventManager.Player.OnReviewFinished.Get().Invoke(reviewCounter, isFailed);

        timer.DisableTimer();

        //Play animation or something that shows what their review was
        yield return new WaitForSeconds(1);

        fillAmountParent.SetActive(false);
    }

    private void OnReviewStart(Vector3 newPosition)
    {
        isFailed = false;
        fillAmountParent.SetActive(true);
        isReviewing = true;
        tickDownReview = true;
        tempReviewScore = Mathf.FloorToInt(reviewCounter);

        var distanceMag = (newPosition - this.transform.position).magnitude;
        var timeAllowed = (distanceMag / 22.5f) + 20f + (economyManager.GetPurchasedUpgradeCount(CarUpgradesEnum.TimerExtender) * 3);
        timer.EnableTimer(timeAllowed);

        SetReviewCounter(-5f);

        var speakerSystemUpgrade = economyManager.GetPurchasedUpgradeCount(CarUpgradesEnum.SpeakerSystem);
        StartCoroutine(StartReviewTicking(speakerSystemUpgrade));
    }
}
