using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ReviewManager : MonoBehaviour
{
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

    private UnityAction reviewStopListener;
    private GameObject fillAmountParent;

    private void Start()
    {
        reviewStopListener = () => StartCoroutine(OnReviewStop());
        EventManager.Player.OnCarCollide.Get().AddListener(OnCarCollided);
        EventManager.Player.OnReviewStop.Get().AddListener(reviewStopListener);
        EventManager.Player.OnReviewStart.Get().AddListener(OnReviewStart);
        EventManager.Player.onCarHonked.Get().AddListener(onCarHonked);
        fillAmountParent = fillAmountController.transform.parent.gameObject;

        StartCoroutine(StartReviewTicking());

        fillAmountParent.SetActive(false);
    }

    public float GetReviewValue() 
        => reviewCounter;

    IEnumerator StartReviewTicking()
    {
        while (tickDownReview && isReviewing)
        {
            yield return new WaitForSeconds(reviewTickDownRate);
            AddToReviewCounter(reviewTickDownAmount);
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
        if (isReviewing)
            AddToReviewCounter(velocity / 100f);
    }

    private void onCarHonked()
    {
        if (isReviewing)
            AddToReviewCounter(0.25f);
    }

    private IEnumerator OnReviewStop()
    {
        isReviewing = false;
        tickDownReview = false;

        //Play animation or something that shows what their review was
        yield return new WaitForSeconds(1);

        fillAmountParent.SetActive(false);
    }

    private void OnReviewStart()
    {
        fillAmountParent.SetActive(true);
        isReviewing = true;
        tickDownReview = true;
        
        SetReviewCounter(-5f);
        StartCoroutine(StartReviewTicking());
    }
}
