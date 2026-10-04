using UnityEngine;
using System.Linq;

public abstract class Core : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public stateMacines stateMacines;

    [Header("Child States (auto-detected, read-only)")]
    [SerializeField] protected State[] childStates;

    public void setupInstances()
    {
        stateMacines = new stateMacines();
        childStates = GetSortedChildStates();

        foreach (State state in childStates)
        {
            state.SetCore(this);
            state.macine = stateMacines;
        }
    }

    // ให้เห็นค่าอัปเดตใน Inspector ทันทีตอนแก้ hierarchy/priority ใน Editor (ไม่ต้องกด Play)
    protected virtual void OnValidate()
    {
        childStates = GetSortedChildStates();
    }

    private State[] GetSortedChildStates()
        => GetComponentsInChildren<State>().OrderByDescending(s => s.priority).ToArray();
}