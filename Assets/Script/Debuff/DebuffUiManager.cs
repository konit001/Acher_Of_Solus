using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DebuffUiManager : MonoBehaviour
{
    [Header("References")]
    public DebuffController targetController;
    public GameObject debuffGagePrefab;
    public Transform container;

    private class DebuffUiEntry
    {
        public Debuff debuff;
        public GameObject instance;
        public Image fillImage;
        public Image iconImage;
    }

    private readonly List<DebuffUiEntry> entries = new List<DebuffUiEntry>();

    private void OnEnable()
    {
        if (targetController != null) targetController.OnDebuffsChanged += RefreshEntries;
        RefreshEntries();
    }

    private void OnDisable()
    {
        if (targetController != null) targetController.OnDebuffsChanged -= RefreshEntries;
    }

    private void Update()
    {
        foreach (DebuffUiEntry entry in entries)
        {
            if (entry.fillImage != null && entry.debuff.debuffEffect.debuffDuration > 0f)
            {
                entry.fillImage.fillAmount = entry.debuff.Duration / entry.debuff.debuffEffect.debuffDuration;
            }
        }
    }

    private void RefreshEntries()
    {
        foreach (DebuffUiEntry entry in entries)
        {
            if (entry.instance != null) Destroy(entry.instance);
        }
        entries.Clear();

        if (targetController == null || debuffGagePrefab == null)
        {
            Debug.LogWarning("[DebuffUI] ขาด reference targetController หรือ debuffGagePrefab");
            return;
        }

        Transform parent = container != null ? container : transform;
        Debug.Log($"[DebuffUI] รีเฟรช HUD: มีดีบัฟ active {targetController.activeDebuffs.Count} ตัว");

        foreach (Debuff debuff in targetController.activeDebuffs)
        {
            GameObject instance = Instantiate(debuffGagePrefab, parent);

            DebuffUiEntry entry = new DebuffUiEntry
            {
                debuff = debuff,
                instance = instance,
                fillImage = instance.transform.Find("Fill")?.GetComponent<Image>(),
                iconImage = instance.transform.Find("Icon")?.GetComponent<Image>()
            };

            if (entry.iconImage != null) entry.iconImage.sprite = debuff.debuffEffect.icon;

            entries.Add(entry);
        }
    }
}
