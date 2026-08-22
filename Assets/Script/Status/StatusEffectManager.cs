using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class StatusEffectEntry
{
    public StatusEffectSO effectSO;
    public StatusEffectType effectType;
    public int Priority;
}

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance;

    public List<StatusEffectEntry> Effect = new List<StatusEffectEntry>();

    public StatusEffectController Player;
    public List<StatusEffectController> Enemy = new List<StatusEffectController>();

    void Awake()
    {
        Instance = this;
    }

    // priorities are authored in the Inspector but nothing reads them yet
    public int GetPriority(StatusEffectSO effectSO)
    {
        foreach (StatusEffectEntry entry in Effect)
            if (entry.effectSO == effectSO) return entry.Priority;
        return 0;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player = playerObject.GetComponent<StatusEffectController>();
        }

        GameObject[] EnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in EnemyObjects)
        {
            StatusEffectController controller = enemyObject.GetComponent<StatusEffectController>();
            if (controller != null) // skip enemies without the component
                Enemy.Add(controller);
        }
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ApplyTest(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ApplyTest(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ApplyTest(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) ApplyTest(3);
    }

    // applies Effect[index] to Player and the first enemy, for quick testing
    private void ApplyTest(int index)
    {
        if (index >= Effect.Count || Effect[index].effectSO == null) return;

        StatusEffectSO effectSO = Effect[index].effectSO;
        Player?.ApplyEffect(effectSO);
        if (Enemy.Count > 0) Enemy[0]?.ApplyEffect(effectSO);
    }
}
