using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LootBagData", menuName = "ItemData/LootBagData")]
public class LootBagData : BaseItemData
{
    public Rarity rarity;
    public List<lootItem> possibleContents = new List<lootItem>();
}
