using UnityEngine;

public static class DontDestroyHelper
{
    public static void MakePersistent(this GameObject go)
    {
        Object.DontDestroyOnLoad(go);
    }
}