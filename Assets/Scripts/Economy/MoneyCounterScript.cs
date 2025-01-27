using TMPro;
using UnityEngine;

public class MoneyCounterScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtMoneyCounter;

    void Start()
    {
        EventManager.Game.OnCashChanged.Get().AddListener(OnCashChanged);
    }

    private void OnCashChanged(int totalCash)
    {
        txtMoneyCounter.text = $"${totalCash}";
    }
}
