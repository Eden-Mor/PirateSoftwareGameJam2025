using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ChatMessageGroup
{
    public string messageGroupIdentifier;
    public List<ChatMessage> messages;
}

[Serializable]
public class ChatMessage
{
    public string name;
    public string message;
}

public class ChatBubbleManagerScript : MonoBehaviour
{
    [SerializeField] private bool isDisplaying = false;
    [SerializeField] private List<ChatMessageGroup> messages = new();
    [SerializeField] private AudioSource swipeSound;

    public List<ChatBubble> bubbles = new();

    private void Start()
    {
        EventManager.ChatBubble.OnAddChat.Get().AddListener((messages) => AddChat(messages));
    }

    public void AddChat(ChatMessageGroup messageGroup)
    {
        messages.Add(messageGroup);
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
            foreach (var chatMessage in messageExchange.messages)
            {
                var bubble = bubbles.First();
                bubbles.Reverse();

                swipeSound.Play();
                bubble.DisplayMessage(chatMessage);
                yield return new WaitForSeconds(2f + chatMessage.message.Count(x => x == ' ') * 0.25f); // Change this later to length of message
                bubble.HideMessage();
            }
            messages.RemoveAt(0);
            EventManager.ChatBubble.OnMessageGroupDisplayed.Get().Invoke(messageExchange.messageGroupIdentifier);

            //After the current message sequence ends, wait 2 seconds before showing the new sequence
            yield return new WaitForSeconds(2);
        }

        isDisplaying = false;
    }
}
