using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GetName : MonoBehaviour
{
    public GameObject namedObject;

    void Start()
    {
        UpdateText();
    }

    void OnValidate()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        var textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.text = namedObject != null ? namedObject.name : "No Object";
    }
}
