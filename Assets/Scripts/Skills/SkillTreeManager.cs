using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;
    public int skillPoints = 0;

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





}
