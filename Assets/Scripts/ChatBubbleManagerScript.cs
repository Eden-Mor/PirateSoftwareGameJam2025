using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ChatMessageGroup
{
    public List<ChatMessage> messages;
    public UnityAction callback;
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
        EventManager.ChatBubble.OnClearQueue.Get().AddListener(OnClearQueue);
    }

    private void OnClearQueue() 
        => messages.Clear();

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
            messageExchange.callback?.Invoke();

            if (messages.Count > 0)
                messages.RemoveAt(0);

            //After the current message sequence ends, wait 2 seconds before showing the new sequence
            yield return new WaitForSeconds(2);
        }

        isDisplaying = false;
    }
}
