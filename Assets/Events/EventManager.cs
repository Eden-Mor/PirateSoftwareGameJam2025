using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CustomerInteraction;

public static class EventManager
{
    public static readonly GameEvents Game = new();
    public static readonly PlayerEvents Player = new();
    public static readonly EconomyEvents Economy = new();
    public static readonly ChatBubbleEvents ChatBubble = new();


    public class PlayerEvents
    {
        //If you need an explanation how this works, check OnCarCollide references
        public class CarCollideEvent : UnityEvent<Component, int> { }
        public GenericEvent<CarCollideEvent> OnCarCollide = new();
        public class ReviewStopEvent : UnityEvent { }
        public GenericEvent<ReviewStopEvent> OnReviewStop = new();
        public class ReviewStartEvent : UnityEvent<Vector3> { }
        public GenericEvent<ReviewStartEvent> OnReviewStart = new();
        public class CarHonkedEvent : UnityEvent { }
        public GenericEvent<CarHonkedEvent> OnCarHonked = new();
        public class ReviewFinishedEvent : UnityEvent<float, bool> { }
        public GenericEvent<ReviewFinishedEvent> OnReviewFinished = new();
    }

    public class GameEvents
    {
        public class TimerCompleteEvent : UnityEvent { }
        public GenericEvent<TimerCompleteEvent> OnTimerComplete = new();
        public class CashChangedEvent : UnityEvent<int> { }
        public GenericEvent<CashChangedEvent> OnCashChanged = new();
    }

    public class EconomyEvents
    {
        public class PurchaseMadeEvent : UnityEvent { }
        public GenericEvent<PurchaseMadeEvent> OnPurchaseMade = new();
    }

    public class ChatBubbleEvents
    {
        public class AddChatEvent : UnityEvent<ChatMessageGroup> { }
        public GenericEvent<AddChatEvent> OnAddChat = new();
        public class ClearQueueEvent : UnityEvent { }
        public GenericEvent<ClearQueueEvent> OnClearQueue = new();
        public class CustomerReactionEvent : UnityEvent<ReactionType> { }
        public GenericEvent<CustomerReactionEvent> OnCustomerReaction = new();
    }
}
