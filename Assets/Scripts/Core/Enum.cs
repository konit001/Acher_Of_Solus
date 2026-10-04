using UnityEngine;

public enum ElementType { None, Fire, Wind, Water, Lightning, Nature, Earth, ElementalLess}
public enum enemyType { Friendly, Neutral, Aggressive }
public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }
public enum ItemType { Coin, Material, Consumable, Artifact, Orb, Weapon, LootBag }
public enum MouseChange { Normal, Attack }
public enum WeaponsType { none, Spear, Gun, Sword, Bow, Staff, Dagger }
public enum ArtifactType { Brooch, Ear, Ring, Scepter }
// แท็บย่อยของหน้า Equipment — ลำดับต้องตรงกับ categoryBar ที่ผูกไว้ใน MenuBarController
public enum EquipmentCategory {Character, Weapon, Artifact }
public enum StatusType { None, Health, Stamina, Attack, ElementalBonus, Defend, CritRate, CritDamage, Speed }


public enum SkillTier { None, One, Two, Three, Four, Five }
public enum ActiveSkillType { Melee, Ranged, AOE, Buff }
public enum SkillNodeStatus { Locked, Available, Unlocked }

public enum StatusEffectType { None, Buff, Debuff }
//Debuff
public enum elementDebuffType { None, poison, Burn, Blisters, Freeze, Faint, Crack, Paralyzed, Shock, Voltex }
public enum GeneralDebuffType { None, Stun, Slow, Weak }

//Buff
public enum elementBuffType { None }
public enum GeneralBuffType { None, Strength, Agility }