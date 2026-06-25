using Photon.Pun;
using UnityEngine;

public class CharacterVisualRoot : MonoBehaviour
{
    // 
    [SerializeField] private CharacterIndicatorRoot _indicatorRoot;

    private RotationHandler _rotationHandler;

    public Vector2 FacingDirection => transform.right;

    public RotationHandler RotationHandler => _rotationHandler = ComponentEnsurer.EnsureComponent(GetComponent<RotationHandler>(), gameObject);

    public CharacterIndicatorRoot IndicatorRoot => _indicatorRoot;
}
