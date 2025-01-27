using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new();
    public static readonly PlayerEvents Player = new();
    public static readonly EconomyEvents Economy = new();

    public class PlayerEvents
    {
        //If you need an explanation how this works, check OnCarCollide references
        public class CarCollideEvent : UnityEvent<Component, int> { }
        public GenericEvent<CarCollideEvent> OnCarCollide = new();
        public class ReviewStopEvent : UnityEvent { }
        public GenericEvent<ReviewStopEvent> OnReviewStop = new();
        public class ReviewStartEvent : UnityEvent { }
        public GenericEvent<ReviewStartEvent> OnReviewStart = new();
        public class CarHonkedEvent : UnityEvent { }
        public GenericEvent<CarHonkedEvent> OnCarHonked = new();
    }

    public class GameEvents
    {
        public class TimerCompleteEvent : UnityEvent { }
        public GenericEvent<TimerCompleteEvent> OnTimerComplete = new();
        public class ResetTimerEvent : UnityEvent { }
        public GenericEvent<TimerCompleteEvent> OnResetTimer = new();
        public class CashChangedEvent : UnityEvent<int> { }
        public GenericEvent<CashChangedEvent> OnCashChanged = new();
    }

    public class EconomyEvents
    {
        public class PurchaseMadeEvent : UnityEvent { }
        public GenericEvent<PurchaseMadeEvent> OnPurchaseMade = new();
    }
}
