using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupScript : MonoBehaviour
{
    public GameObject pickupDropoffPortal;
    [SerializeField] private bool skipStoryline = false;
    void OnEnable()
    {
        StartCoroutine(StartStoryline());
    }

    private IEnumerator StartStoryline()
    {
        if (!skipStoryline)
            yield return new WaitForSeconds(2);

        var messages = new List<ChatMessage>()
        {
            new(){name = "Boss", message="Hey...."},
            new(){name = "You", message="Yes boss? You sound concerned, is everything all right?"},
            new(){name = "Boss", message="I wanted to talk to you about your position."},
            new(){name = "Boss", message="Big boss wants to incorporate 'AI' into our company, we no longer need drivers at Mesla Inc."},
            new(){name = "You", message="Not again! I am through with all this AI bull$@#%, I am done with this, if I'm going down..."},
            new(){name = "You", message="SO IS MESLA INC."},
        };

        var messageGroup = new ChatMessageGroup()
        {
            messages = messages,
            callback = () =>
            {
                SetupGame();
                EventManager.ChatBubble.OnAddChat.Get().Invoke(new ChatMessageGroup
                {
                    messages = new()
                    {
                        new () { name="Tutorial", message="Use the arrow to head to the dropoff and pickup points.    "},
                        new () { name="Tutorial", message="Get bad reviews to take down Mesla Inc.    "},
                        new () { name="Tutorial", message="You must make a full stop for passengers to get on or get off.    "},
                        new () { name="Tutorial", message="WASD to move, Space to use your hand brake, Shift to boost (when unlocked).   "}
                    }
                });
            }
        };

        if (skipStoryline)
            SetupGame();
        else
            EventManager.ChatBubble.OnAddChat.Get().Invoke(messageGroup);
    }

    private void SetupGame()
    {
        pickupDropoffPortal.SetActive(true);
    }
}
