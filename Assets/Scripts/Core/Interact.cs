using UnityEngine;

public class Interact : MonoBehaviour
{
    public bool canInteract;
    public float radius;
    public LayerMask interactLayer;
    void Start()
    {
        
    }

    public bool isInteract()
    {
        return canInteract = Physics2D.OverlapCircle(this.transform.position , radius , interactLayer);
    }
    void Update()
    {
        if(isInteract() == true)
        {
            // check if it is Chest show a Ui button to make a player actknowless hot to interact with it 
            // if it a NPC start a dialohue UI
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position , radius);
    }


}
