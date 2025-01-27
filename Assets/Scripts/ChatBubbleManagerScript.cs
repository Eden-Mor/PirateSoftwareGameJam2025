using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ChatMessage
{
    public string name;
    public string message;
}

public class ChatBubbleManagerScript : MonoBehaviour
{
    [SerializeField] private bool isDisplaying = false;
    [SerializeField] private List<List<ChatMessage>> messages = new();
    
    public List<ChatBubble> bubbles = new();

    public void DEBUG()
    {
        AddChat(new List<ChatMessage> { new() { name = "Person A", message = "Ring ring." },
        new() { name = "Jake", message = "Ill be on in a sec." },
        new() { name = "Person B", message = "You said that last time ive been waiting for a while :( " },
        new() { name = "Jake", message = "Ugh fine ill come on like I promised" }});
    }

    public void AddChat(List<ChatMessage> message)
    {
        messages.Add(message);
        StartCoroutine(StartMessageExchange());
    }

    private IEnumerator StartMessageExchange()
    {
        if (isDisplaying)
            yield break;

        isDisplaying = true;

        while (messages.Count > 0)
        {
            var messageExchange = messages.First();
            foreach (var chatMessage in messageExchange)
            {
                var bubble = bubbles.First();
                bubbles.Reverse();

                bubble.DisplayMessage(chatMessage);
                yield return new WaitForSeconds(3); // Change this later to length of message
                bubble.HideMessage();
            }
            messages.RemoveAt(0);

            //After the current message sequence ends, wait 2 seconds before showing the new sequence
            yield return new WaitForSeconds(2);
        }

        isDisplaying = false;
    }
}
