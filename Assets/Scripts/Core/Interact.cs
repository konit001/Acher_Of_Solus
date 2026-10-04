using UnityEngine;
using Player.Input;

public class Interact : MonoBehaviour
{
    public bool canInteract;
    public float radius;
    public LayerMask interactLayer;

    [Header("Interact Prompt")]
    public GameObject interactPromptPrefab;   // ลาก prefab ไอคอน (มี SpriteRenderer) จาก Project มาใส่
    public Vector3 promptOffset = Vector3.up;

    private GameObject activePrompt;
    private Collider2D currentTarget;

    void Update()
    {
        currentTarget = FindNearestInteractable();
        canInteract = currentTarget != null;

        UpdatePrompt();

        if (canInteract && userInput.instance.interactInput)
        {
            IInteractable interactable = currentTarget.GetComponent<IInteractable>()
                ?? currentTarget.GetComponentInParent<IInteractable>();
            interactable?.OnInteract(gameObject);
        }
    }

    private void UpdatePrompt()
    {
        if (interactPromptPrefab == null) return;

        if (canInteract)
        {
            Vector3 promptPosition = currentTarget.transform.position + promptOffset;

            if (activePrompt == null || !activePrompt.activeInHierarchy)
            {
                activePrompt = ObjectPooling.Instance.SpawnFromPool(interactPromptPrefab, promptPosition, Quaternion.identity);
            }
            else
            {
                activePrompt.transform.position = promptPosition;
            }
        }
        else if (activePrompt != null)
        {
            activePrompt.SetActive(false);
            activePrompt = null;
        }
    }

    private Collider2D FindNearestInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, interactLayer);
        Collider2D nearest = null;
        float nearestSqrDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            float sqrDist = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = hit;
            }
        }
        return nearest;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
