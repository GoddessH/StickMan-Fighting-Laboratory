using Photon.Pun;
using UnityEngine;

public class CharacterVisualRoot : MonoBehaviour
{
    // 
    [SerializeField] private CharacterIndicatorRoot _indicatorRoot;

    private AnimationHandler _animationHandler;
    private RotationHandler _rotationHandler;

    public AnimationHandler AnimationHandler => _animationHandler = ComponentEnsurer.EnsureComponent(GetComponent<AnimationHandler>(), gameObject);
    public RotationHandler RotationHandler => _rotationHandler = ComponentEnsurer.EnsureComponent(GetComponent<RotationHandler>(), gameObject);

    public CharacterIndicatorRoot IndicatorRoot => _indicatorRoot;
}
