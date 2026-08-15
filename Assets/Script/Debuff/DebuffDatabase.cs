using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DebuffDatabase", menuName = "Managers/DebuffDatabase")]
public class DebuffDatabase : ScriptableObject
{
    [System.Serializable]
    public class ElementDebuffSet
    {
        public ElementType element;
        public List<DebuffEffect> debuffs = new List<DebuffEffect>();
    }

    [SerializeField] private List<ElementDebuffSet> table = new List<ElementDebuffSet>();
    private Dictionary<ElementType, List<DebuffEffect>> lookup;

    private void Init()
    {
        lookup = new Dictionary<ElementType, List<DebuffEffect>>();
        foreach (ElementDebuffSet set in table)
            lookup[set.element] = set.debuffs;
    }

    public DebuffEffect GetRandomDebuff(ElementType element)
    {
        if (lookup == null) Init();

        if (!lookup.TryGetValue(element, out List<DebuffEffect> debuffs) || debuffs == null || debuffs.Count == 0)
        {
            Debug.LogWarning($"[DebuffDatabase] ไม่พบ DebuffEffect ที่ผูกกับธาตุ {element} ใน table");
            return null;
        }

        DebuffEffect picked = debuffs[Random.Range(0, debuffs.Count)];
        Debug.Log($"[DebuffDatabase] สุ่มธาตุ {element} ได้ '{picked.debuffName}' (จากทั้งหมด {debuffs.Count} ตัวเลือก)");
        return picked;
    }
}
