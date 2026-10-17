using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        gameObject.MakePersistent();
    }
}
