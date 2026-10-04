using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public int CoinCount;
    private int MaxCoinCount = 999;
    public TextMeshProUGUI coinText;

    // ยิงทุกครั้งที่เหรียญเปลี่ยน — ให้ปุ่ม Upgrade ในแผง Equipment รีเฟรชสถานะตาม
    public event System.Action OnCoinChanged;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

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
        OnCoinChanged?.Invoke();
    }

    public bool HasCoin(int amount)
    {
        return CoinCount >= amount;
    }

    public void SpendCoin(int amount)
    {
        CoinCount = Mathf.Max(0, CoinCount - amount);
        UpdateUI();
        OnCoinChanged?.Invoke();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = CoinCount.ToString();
        }
    }
}
