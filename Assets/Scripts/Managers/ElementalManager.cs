using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ElementalManager", menuName = "Managers/ElementalManager")]
public class ElementalManager : ScriptableObject
{
    public enum MatchupResult { Weak = 0, Normal = 1, Strong = 2 }

    [System.Serializable]
    public class ElementalMatchup
    {
        public ElementType attacker;
        public ElementType defender;
        public MatchupResult result = MatchupResult.Normal;

        public float Multiplier()
        {
            switch (result)
            {
                case MatchupResult.Weak: return 0.5f;
                case MatchupResult.Strong: return 1.5f;
                default: return 1.0f;
            }
        }
    }

    [SerializeField] private List<ElementalMatchup> matchups = new List<ElementalMatchup>();
    private Dictionary<(ElementType, ElementType), float> table;

    public void Init()
    {
        table = new Dictionary<(ElementType, ElementType), float>();
        foreach (var m in matchups)
            table[(m.attacker, m.defender)] = m.Multiplier();
    }

    public float GetMultiplier(ElementType attacker, ElementType defender)
    {
        if (table == null) Init();
        return table.TryGetValue((attacker, defender), out float mult) ? mult : 1f;
    }

    public static Color GetElementColor(ElementType element)
    {
        string hex;

        switch (element)
        {
            case ElementType.Fire:
                hex = "#FF6600";
                break;
            case ElementType.Water:
                hex = "#00AAFF";
                break;
            case ElementType.Nature:
                hex = "#33CC33";
                break;
            case ElementType.Lightning:
                hex = "#FFE600";
                break;
            case ElementType.Earth:
                hex = "#996633";
                break;
            case ElementType.Wind:
                hex = "#99DDFF";
                break;
            default:
                hex = "#FFFFFF";
                break;
        }

        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
}

