using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerInteraction : MonoBehaviour
{
    public enum ReactionType
    {
        CarCollision,
        StarLost,
        StarGained,
        CustomerGotOn,
        CustomerGotOff,
    }

    private void Start()
    {
        EventManager.ChatBubble.OnCustomerReaction.Get().AddListener(OnCustomerReaction);
    }

    private readonly List<ChatMessageGroup> carHitReactions = new()
    {
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="Oh my god, please drive more carefully" },
                new ChatMessage(){ name="You", message="Sorry, I fell asleep." },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="CHESUS, please watch yourself." },
                new ChatMessage(){ name="You", message="Excuse me, I am part of the spagetti monster believers." },
                new ChatMessage(){ name="Customer", message="Oh cheers! Me too." },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="You're definitely getting a bad welp review." },
                new ChatMessage(){ name="You", message="NOOOO, Anything but that! Please dont give do that!" },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="SERIOUSLY, please drive safe!" },
                new ChatMessage(){ name="You", message="Nahhh im good." },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="Wow, you really smashed into that!" },
                new ChatMessage(){ name="You", message="Dont tempt me with a good time." },
            }
        }
    };

    private readonly List<ChatMessageGroup> starLostReactions = new()
    {
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="I dont like the way you drive..." },
                new ChatMessage(){ name="You", message="Oh yeah? fine, im letting go of the wheel, how about you drive?" },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="Do you drive like this every time???" },
                new ChatMessage(){ name="You", message="Would you like my personal number? I wouldnt mind driving you again!" },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="If you continue causing problems on the road, I will write a bad review about you." },
                new ChatMessage(){ name="You", message="Sounds great! thank you very much!" },
            }
        },
    };

    private readonly List<ChatMessageGroup> passengerGotOn = new()
    {
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="Hey how are you doing." },
                new ChatMessage(){ name="You", message="Terrible, dont blame me for what happens next" },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="..." },
                new ChatMessage(){ name="You", message="..." },
                new ChatMessage(){ name="You", message="You alright mate?" },
                new ChatMessage(){ name="Customer", message="..." },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="You", message="Terrible morning to you!" },
                new ChatMessage(){ name="Customer", message="Whats wrong with you?" },
            }
        },
    };

    private readonly List<ChatMessageGroup> passengerGotOff = new()
    {
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="Have a better day... I need some wine." },
                new ChatMessage(){ name="You", message="I just had some myself!" },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="I am never getting another ride from you" },
                new ChatMessage(){ name="You", message="Perfect!" },
            }
        },
        new ()
        {
            messages = new()
            {
                new ChatMessage(){ name="Customer", message="That was actually quite pleasant." },
                new ChatMessage(){ name="You", message="You !@#$ing !@#$%." },
                new ChatMessage(){ name="Customer", message="Alright nevermind." },
            }
        },
    };

    private float cooldownTime = 10f;
    private float lastCalledCollisionTime = -15f;
    private float lastCalledStarLostTime = -15f;
    private void OnCustomerReaction(ReactionType reactionType)
    {
        var randomVal = Random.Range(0f, 1f);
        switch (reactionType)
        {
            case ReactionType.CarCollision:
                if (randomVal < 0.3f && Time.time > lastCalledCollisionTime + cooldownTime)
                {
                    lastCalledCollisionTime = Time.time;
                    EventManager.ChatBubble.OnAddChat.Get().Invoke(carHitReactions.ElementAt(UnityEngine.Random.Range(0, carHitReactions.Count)));
                }
                break;
            case ReactionType.StarLost:
                if (randomVal < 0.1f && Time.time > lastCalledStarLostTime + cooldownTime)
                {
                    lastCalledStarLostTime = Time.time;
                    EventManager.ChatBubble.OnAddChat.Get().Invoke(starLostReactions.ElementAt(UnityEngine.Random.Range(0, starLostReactions.Count)));
                }
                break;
            case ReactionType.CustomerGotOn:
                if (randomVal < 0.5f)
                    EventManager.ChatBubble.OnAddChat.Get().Invoke(passengerGotOn.ElementAt(UnityEngine.Random.Range(0, passengerGotOn.Count)));
                break;
            case ReactionType.CustomerGotOff:
                if (randomVal < 0.5f)
                    EventManager.ChatBubble.OnAddChat.Get().Invoke(passengerGotOff.ElementAt(UnityEngine.Random.Range(0, passengerGotOff.Count)));
                break;
        }
    }
}
