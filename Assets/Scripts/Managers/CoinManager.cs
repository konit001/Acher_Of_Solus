using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int CoinCount;
    private int MaxCoinCount = 999;
    public TextMeshProUGUI coinText;
    
    void Start()
    {
        CoinCount = 0;
        UpdateUI();
    }
    public void AddCoin(int amount = 1)
    {
        CoinCount += amount;
        CoinCount = Mathf.Clamp(CoinCount, 0, MaxCoinCount);
        
        UpdateUI(); 
    }

    public bool HasCoin(int amount)
    {
        return CoinCount >= amount;
    }

    public void SpendCoin(int amount)
    {
        CoinCount = Mathf.Max(0, CoinCount - amount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = CoinCount.ToString();
        }
    }
}
