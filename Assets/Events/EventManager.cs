using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly GameEvents Game = new();

    public class GameEvents
    {
        public class TimerCompleteEvent : UnityEvent { }
        public GenericEvent<TimerCompleteEvent> OnTimerComplete = new();
        public class ResetTimerEvent : UnityEvent { }
        public GenericEvent<TimerCompleteEvent> OnResetTimer = new();

        //HOW TO USE, How to subscribe, how to invoke, how to unsubscribe, how to add filters.Get("Filter")
        //EventManager.Game.OnTimerComplete.Get().Invoke(this, 1);
        //private void OnEnable()
        //{
        //    EventManager.Player.OnHealthChanged.Get().AddListener(UpdateHealth);
        //}
        //private void OnDisable()
        //{
        //    EventManager.Player.OnHealthChanged.Get().AddListener(UpdateHealth);
        //}
        //private void UpdateHealth(Component component, int health)
        //{
        //    label.SetText(health.ToString());
        //}
    }
}
