using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{

    public static LoadingScreen Instance;
    public GameObject loadingOverlay;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // --------------------------- //
    public void SwitchToLevel(int sceneID, GameState newScene)
    {
        GameManager.Instance.ChangeGameState(GameState.Loading);
        StartCoroutine(SwitchToSceneAsync(sceneID, newScene));  
    }

    // --------------------------- //
    IEnumerator SwitchToSceneAsync(int sceneID, GameState newScene)
    {
        loadingOverlay.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);
        asyncLoad.allowSceneActivation = false;

        // Ai brukt for å finne ut av WaitForSecondsRealtime var en ting
        yield return new WaitForSecondsRealtime(1.5f);

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        loadingOverlay.SetActive(false);
        GameManager.Instance.ChangeGameState(newScene);
    }
}
