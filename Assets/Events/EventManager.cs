using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static readonly PlayerEvents Player = new();

    public class PlayerEvents
    {
        //public class HealthEvent : UnityEvent<Component, int> { }
        //public GenericEvent<HealthEvent> OnHealthChanged = new();

        //HOW TO USE, How to subscribe, how to invoke, how to unsubscribe, how to add filters .Get("Filter")
        //EventManager.Player.OnHealthChanged.Get().Invoke(this, 1);
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
