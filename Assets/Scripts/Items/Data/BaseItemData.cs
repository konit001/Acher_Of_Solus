using UnityEngine;

public class BaseItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea(3, 5)] public string description;
    public ItemType itemType;
    public Sprite itemImage;
}
