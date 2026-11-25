using UnityEngine;
using TMPro; // Обратите внимание на эту строку

public class UIManager : MonoBehaviour
{
    public TMP_Text coinText; // Тип изменен на TMP_Text

    void Update()
    {
        if (GameManager.Instance != null && coinText != null)
        {
            coinText.text = "Монеты: " + GameManager.Instance.totalCoins;
        }
    }
}