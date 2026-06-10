using Photon.Pun;
using UnityEngine;

public class SingletonPun<T> : Singleton<T> where T : MonoBehaviour
{
    // 
    private static PhotonView _photonView;
    protected PhotonView photonView
    {
        get
        {
            _photonView = ComponentEnsurer.EnsureComponent(GetComponent<PhotonView>(), gameObject);
            return _photonView;
        }
    }
}
