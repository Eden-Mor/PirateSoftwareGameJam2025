using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupScript : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(StartStoryline());
    }

    private IEnumerator StartStoryline()
    {
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

        EventManager.ChatBubble.OnAddChat.Get().Invoke(messages);
    }
}
