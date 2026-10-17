using UnityEngine;
using System.Linq;

public abstract class Core : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public stateMacines stateMacines;
    public stateMacines AdditiveMachine;

    [Header("Child States (auto-detected, read-only)")]
    [SerializeField] protected State[] childStates;

    public void setupInstances()
    {
        stateMacines = new stateMacines();
        AdditiveMachine = new stateMacines();

        stateMacines.layer = StateLayer.Base;
        AdditiveMachine.layer = StateLayer.Additive;

        childStates = GetSortedChildStates();

        foreach (State state in childStates)
        {
            state.SetCore(this);
            state.macine = (state.layer == StateLayer.Base) ? stateMacines : AdditiveMachine;
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