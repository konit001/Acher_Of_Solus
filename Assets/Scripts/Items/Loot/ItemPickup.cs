using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
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
        if(other.CompareTag("Player") && (itemData.itemType == ItemType.Coin || itemData.autoCollect))
        {
            Collect();
        }
    }

    public void OnInteract(GameObject interactor)
    {
        if(itemData.itemType != ItemType.Coin && !itemData.autoCollect)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if(itemData.itemType == ItemType.Coin)
        {
            coinManager.AddCoin();
            audioManager.Instance.PlayeSFX("Coin");
        }
        UiManager.instance.CollectItem(itemData);
        Destroy(gameObject);
    }
}
