using UnityEngine;

[System.Serializable]
public class lootItem
{
    public BaseItemData itemData;
    public GameObject dropPrefab;
    [Range(0f, 100f)] public int dropChance;
    [Range(1f,50f)] public int dropAmount;
}
