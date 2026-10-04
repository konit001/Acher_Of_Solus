using UnityEngine;

public class BaseItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea(3, 5)] public string description;
    public ItemType itemType;
    public Sprite itemImage;
    public bool autoCollect; // เก็บอัตโนมัติเมื่อเดินชน เหมือนเหรียญ ไม่ต้องกด Interact
}
