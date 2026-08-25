using UnityEngine;

public class ArtifactSlot : SlotBase
{
    public ArtifactType artifactType;

    protected override void Awake()
    {
        base.Awake();
        Clear();
    }

    protected override void OnRightClickInMenu()
    {
        UiManager.instance.UnEquipArtifact(this);
    }
}
