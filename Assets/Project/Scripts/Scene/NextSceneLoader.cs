using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    // 
    [SerializeField] private SceneType _nextScene;
    [SerializeField] private JoinRoomHandler _joinRoomHandler;

    #region Call in Button's event
    public void LoadNextScene()
    {
        int nextSceneIndex = (int)_nextScene;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (nextSceneIndex < 0 || nextSceneIndex >= sceneCount) return;

        if (_joinRoomHandler == null) SceneManager.LoadSceneAsync(nextSceneIndex);
        else _joinRoomHandler.ConnectToMasterServer(() => PhotonNetwork.LoadLevel(nextSceneIndex));
    }
    #endregion
}
