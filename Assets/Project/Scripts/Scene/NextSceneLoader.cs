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
        int currentSceneIndex = (int)_nextScene;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex < 0 || currentSceneIndex >= sceneCount) return;

        PhotonNetwork.LoadLevel((int)_nextScene);
    }
    #endregion
}
