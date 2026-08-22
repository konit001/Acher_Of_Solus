using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public BaseItemData itemData;
    private CoinManager coinManager;
    public void Setup(BaseItemData item)
    {
        itemData = item;
    }
    void Start()
    {
        coinManager = FindFirstObjectByType<CoinManager>(); 
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(itemData.itemType == ItemType.Coin)
            {
                coinManager.AddCoin();
                audioManager.Instance.PlayeSFX("Coin");
            }
            Destroy(gameObject);
            UiManager.instance.AddItem(itemData);
        }
    }
}
