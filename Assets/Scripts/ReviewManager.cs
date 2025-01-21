using System;
using System.Collections;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{
    public FillAmountController fillAmountController;

    [Range(-1f, 0f), SerializeField]
    private float reviewTickDownAmount = -0.1f;

    [Range(0, 10), SerializeField]
    private int reviewTickDownRate = -1;

    [Range(-5f, 5f), SerializeField]
    private float reviewCounter = 0f;

    [SerializeField]
    private bool tickDownReview = true;

    private void Start()
    {
        StartCoroutine(StartReviewTicking());
    }

    private void OnEnable()
    {
        EventManager.Player.OnCarCollide.Get().AddListener(OnCarCollided);
        EventManager.Player.OnReviewStop.Get().AddListener(OnReviewStop);
        EventManager.Player.OnReviewStart.Get().AddListener(OnReviewStart);
    }

    private void OnDisable()
    {
        EventManager.Player.OnCarCollide.Get().AddListener(OnCarCollided);
        EventManager.Player.OnReviewStop.Get().AddListener(OnReviewStop);
        EventManager.Player.OnReviewStart.Get().AddListener(OnReviewStart);
    }

    public float GetReviewValue() 
        => reviewCounter;

    IEnumerator StartReviewTicking()
    {
        while (tickDownReview)
        {
            yield return new WaitForSeconds(reviewTickDownRate);
            AddToReviewCounter(reviewTickDownAmount);
        }
    }

    private void AddToReviewCounter(float amount)
    {
        reviewCounter += amount;
        fillAmountController.UpdateFillAmounts(reviewCounter);
    }

    private void OnCarCollided(Component component, int velocity)
    {
        // in the future check what component it was colliding with to determine better values for decreasing the review count;
        AddToReviewCounter(velocity / 100f);
    }

    private void OnReviewStop()
    {
        tickDownReview = false;
    }

    private void OnReviewStart()
    {
        reviewCounter = 0f;
        tickDownReview = true;
        StartCoroutine(StartReviewTicking());
    }
}
