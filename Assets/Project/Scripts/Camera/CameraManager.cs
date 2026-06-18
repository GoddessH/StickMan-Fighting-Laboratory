using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 
    [SerializeField] private CinemachineCamera _cinemachineCamera;

    public void SetCameraTarget(Transform target)
    {
        if (_cinemachineCamera == null) return;

        _cinemachineCamera.Follow = target;
        Debug.Log("End");
    }
}
