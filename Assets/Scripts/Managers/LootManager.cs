using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager instance;

    private CoinManager coinManager;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        coinManager = FindFirstObjectByType<CoinManager>();
    }

    // ─── Public API ───────────────────────────────────────────

    public void OpenLootBag(BaseItemData lootBagItem)
    {
        var lootBagData = lootBagItem as LootBagData;
        if (lootBagData == null || lootBagData.possibleContents.Count == 0)
        {
            Debug.LogWarning("LootBag ไม่มีของให้สุ่ม");
            return;
        }

        foreach (var loot in lootBagData.possibleContents)
        {
            if (!RollDrop(loot.dropChance)) continue;

            GiveLoot(loot);
        }

        UiManager.instance.RemoveItem(lootBagItem);
    }

    // ─── Private ──────────────────────────────────────────────

    private bool RollDrop(float chance)
    {
        return Random.Range(0f, 100f) <= chance;
    }

    private void GiveLoot(lootItem loot)
    {
        for (int i = 0; i < loot.dropAmount; i++)
            UiManager.instance.CollectItem(loot.itemData);

        if (loot.itemData.itemType == ItemType.Coin)
            coinManager.AddCoin(loot.dropAmount);
    }
}
