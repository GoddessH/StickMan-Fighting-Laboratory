using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    // 
    [SerializeField] private SceneType _nextScene;

    #region Call in Button's event
    public void LoadNextScene()
    {
        int nextSceneIndex = (int)_nextScene;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (nextSceneIndex < 0 || nextSceneIndex >= sceneCount) return;

        if (_nextScene == SceneType.Trainning) SceneManager.LoadSceneAsync(nextSceneIndex);
        else PhotonNetwork.LoadLevel(nextSceneIndex);
    }
    #endregion
}
