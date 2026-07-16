using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : Singleton<SceneFlowManager>
{
    [Header("Scene Names")]
    [SerializeField] private string _lobbySceneName = "Lobby";

    private IEnumerator LoadSceneRoutine(string sceneName, Action onLoaded = null)
    {
        Time.timeScale = 1f; // 배속 초기화
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneFlowManager: sceneName is null or empty.");
                yield break;
        }

        ObjectPoolManager.Instance.ReturnAllActiveObject();

        SoundManager.Instance.StopBGM(1f);

        yield return CommonUIManager.Instance.LoadingUI.ShowLoadingUI();
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
            yield return null;

        yield return new WaitForSeconds(2f);

        onLoaded?.Invoke();
        yield return CommonUIManager.Instance.LoadingUI.HideLoadingUI();
    }

    public void LoadScene(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"SceneFlowManager: buildIndex {buildIndex} is out of range.");
            return;
        }

        SceneManager.LoadScene(buildIndex);
    }

    public void ReloadCurrentScene(Action onLoaded = null)
    {
        StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name, onLoaded));
    }

    public void LoadStageScene(int selectStage, Action onLoaded = null)
    {
        string selectSceneName = "Stage" + selectStage;

        if (Application.CanStreamedLevelBeLoaded(selectSceneName))
        {
            StartCoroutine(LoadSceneRoutine(selectSceneName, onLoaded));
        }
        else
        {
            Debug.Log("No next stage. Returning to lobby.");
            LoadLobbyScene();
        }
    }
    
    public void LoadLobbyScene(Action onLoaded = null)
    {
        // Lobby로 씬 전환 시 Stage 정리
        StageManager.Instance.CleanupStage();
        StartCoroutine(LoadSceneRoutine(_lobbySceneName, onLoaded));
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
