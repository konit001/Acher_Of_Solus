using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WarpManager : MonoBehaviour
{
    public string warpSceneName;
    [Header("Player Pos")]
    public Vector2 playerPos;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine();
        }
    }
    void playerPosition()
    {
        playerPos.x = playerControl.instance.transform.position.x;
        playerPos.y = playerControl.instance.transform.position.y;
    }

    IEnumerator StartCoroutine()
    {
        playerPosition();
        FadeManager.instance.LoadSceneWithFade(warpSceneName);
        SceneManager.LoadScene(warpSceneName);
        yield return null;
    }
}