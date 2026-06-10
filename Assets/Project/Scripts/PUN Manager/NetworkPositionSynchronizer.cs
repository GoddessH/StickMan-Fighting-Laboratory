using UnityEngine;
using Photon.Pun;

public class NetworkPositionSynchronizer : MonoBehaviourPun, IPunObservable
{
    //
    private const float _interpolationRate = .1f;
    private Vector2 _networkedPosition;

    /// <summary>
    /// Execute only other client
    /// </summary>
    private void Update()
    {
        if (photonView.IsMine) return;

        transform.position = Vector2.Lerp(transform.position, _networkedPosition, _interpolationRate);
    }

    #region Explicit implement IPunObservable
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((Vector2)transform.position);
        }
        else
        {
            _networkedPosition = (Vector2)stream.ReceiveNext();
        }
    }
    #endregion
}
