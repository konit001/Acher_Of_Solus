using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuffDatabase", menuName = "Managers/BuffDatabase")]
public class BuffDatabase : ScriptableObject
{
    [System.Serializable]
    public class ElementBuffSet
    {
        public ElementType element;
        public List<BuffEffect> buffs = new List<BuffEffect>();
    }

    [SerializeField] private List<ElementBuffSet> table = new List<ElementBuffSet>();
    private Dictionary<ElementType, List<BuffEffect>> lookup;

    private void Init()
    {
        lookup = new Dictionary<ElementType, List<BuffEffect>>();
        foreach (ElementBuffSet set in table)
            lookup[set.element] = set.buffs;
    }

    public BuffEffect GetRandomBuff(ElementType element)
    {
        if (lookup == null) Init();

        if (!lookup.TryGetValue(element, out List<BuffEffect> buffs) || buffs == null || buffs.Count == 0)
            return null;

        return buffs[Random.Range(0, buffs.Count)];
    }
}
