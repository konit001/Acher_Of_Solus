using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    // อยู่ในไฟล์เดียวกันโดยตั้งใจ — ใช้เฉพาะภายใน SkillTreeManager เท่านั้น ไม่มีที่อื่นอ้างอิง
    public class SkillState
    {
        public SkillEffect skill;
        public SkillNodeStatus status;
    }

    public static SkillTreeManager Instance;

    #region Skill Point Economy
    public int skillPoints = 0;

    public void AddSkillPoint(int amount)
    {
        skillPoints += amount;
    }

    public bool CanUnlock(SkillEffect skill)
    {
        SkillState state = GetState(skill);
        if (state == null) return false;
        if (state.status != SkillNodeStatus.Available) return false;

        return skillPoints >= skill.skillPointCost;
    }

    public bool TryUnlock(SkillEffect skill)
    {
        if (!CanUnlock(skill)) return false;

        SkillState state = GetState(skill);
        skillPoints -= skill.skillPointCost;
        state.status = SkillNodeStatus.Unlocked;

        RefreshAllStatuses();
        return true;
    }
    #endregion

    #region Registry
    [Header("Passive Skill")]
    public List<PassiveSkillData> passiveSkills = new List<PassiveSkillData>();

    [Header("Active Skill")]
    public List<ActiveSkillData> activeSkills = new List<ActiveSkillData>();

    private readonly List<SkillState> skillStates = new List<SkillState>();
    private readonly Dictionary<SkillEffect, SkillState> stateLookup = new Dictionary<SkillEffect, SkillState>();
    private readonly Dictionary<int, SkillState> stateByIdLookup = new Dictionary<int, SkillState>();
    public IReadOnlyList<SkillState> SkillStates => skillStates;

    private void RegisterSkill()
    {
        foreach (ActiveSkillData activeSkill in activeSkills)
            AddSkill(activeSkill);

        foreach (PassiveSkillData passiveSkill in passiveSkills)
            AddSkill(passiveSkill);
    }

    private void AddSkill(SkillEffect skill)
    {
        if (skill == null) return;
        if (stateLookup.ContainsKey(skill)) return;

        SkillState state = new SkillState
        {
            skill = skill,
            status = SkillNodeStatus.Locked
        };

        skillStates.Add(state);
        stateLookup.Add(skill, state);
        stateByIdLookup[skill.skillId] = state;
    }

    public SkillState GetState(SkillEffect skill)
    {
        if (skill == null) return null;
        return stateLookup.TryGetValue(skill, out SkillState state) ? state : null;
    }

    public SkillState GetState(int skillId)
    {
        return stateByIdLookup.TryGetValue(skillId, out SkillState state) ? state : null;
    }

    public List<SkillState> GetStatesByTier(SkillTier tier)
    {
        List<SkillState> result = new List<SkillState>();

        foreach (SkillState state in skillStates)
            if (state.skill.tier == tier) result.Add(state);

        return result;
    }

    public float GetTotalPassiveBonus(StatusType stat)
    {
        float total = 0f;

        foreach (SkillState state in skillStates)
        {
            if (state.status != SkillNodeStatus.Unlocked) continue;

            if (state.skill is PassiveSkillData passive)
                total += passive.GetBonusPercent(stat);
        }

        return total;
    }
    #endregion

    #region Prerequisite Graph
    private void RefreshAllStatuses()
    {
        foreach (SkillState state in skillStates)
        {
            if (state.status == SkillNodeStatus.Unlocked) continue;

            state.status = IsPrerequisite(state.skill) ? SkillNodeStatus.Available : SkillNodeStatus.Locked;
        }
    }

    private bool IsPrerequisite(SkillEffect skill)
    {
        SkillEffect required = skill.requiredPreviousSkill;
        if (required == null) return true;
        return IsUnlocked(required);
    }

    public bool IsUnlocked(SkillEffect skill)
    {
        SkillState state = GetState(skill);
        return state != null && state.status == SkillNodeStatus.Unlocked;
    }
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        RegisterSkill();
        RefreshAllStatuses();
    }

#if UNITY_EDITOR
    [ContextMenu("Debug/Log Skill States")]
    private void LogSkillStates()
    {
        Debug.Log($"[SkillTree] แต้ม {skillPoints} | ช่องทั้งหมด {skillStates.Count}");

        foreach (SkillState state in skillStates)
            Debug.Log($"  {state.skill.skillName} (tier {state.skill.tier}) → {state.status}");
    }

    [ContextMenu("Debug/Add 10 Skill Points")]
    private void AddTestPoints() => AddSkillPoint(10);
#endif
}
