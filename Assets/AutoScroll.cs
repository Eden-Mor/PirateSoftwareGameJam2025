using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] bool isAutoScrolling = true;
    
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = this.GetComponent<ScrollRect>();
    }

    void Update()
    {
        if (!isAutoScrolling || scrollRect.verticalNormalizedPosition < 0)
            return;

        scrollRect.verticalNormalizedPosition -= 0.05f * Time.deltaTime;
    }
}
