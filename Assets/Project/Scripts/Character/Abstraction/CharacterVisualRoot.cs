using UnityEngine;

public class CharacterVisualRoot : MonoBehaviour
{
    // 
    private AnimationHandler _animationHandler;
    private RotationHandler _rotationHandler;

    public AnimationHandler AnimationHandler => _animationHandler = ComponentEnsurer.EnsureComponent(GetComponent<AnimationHandler>(), gameObject);
    public RotationHandler RotationHandler => _rotationHandler = ComponentEnsurer.EnsureComponent(GetComponent<RotationHandler>(), gameObject);

    private void Awake()
    {
        _rotationHandler = GetComponent<RotationHandler>();
    }
}
