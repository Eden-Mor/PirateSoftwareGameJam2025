using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI personName;
    [SerializeField] private TextMeshProUGUI personText;
    [SerializeField] private Image personSprite;

    private RectTransform rectTransform;
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetStartLocation();
    }

    private void SetStartLocation()
    {
        gameObject.transform.position = new(0f, -rectTransform.rect.height, 0f);
    }

    public void SetDetails(string name, string text)
    {
        personName.text = name;
        personText.text = text;
    }

    public void DisplayMessage(ChatMessage chatMessage)
    {
        this.gameObject.SetActive(true);
        SetDetails(chatMessage.name, chatMessage.message);
        LeanTween.move(this.gameObject, new Vector3(0f, 0f, 0f), 0.5f);
    }

    public void HideMessage()
    {
        LeanTween.move(this.gameObject, new Vector3(-rectTransform.rect.width, 0, 0f), 0.5f).setOnComplete(() =>
        {
            SetStartLocation();
            this.gameObject.SetActive(false);
        });
    }
}
