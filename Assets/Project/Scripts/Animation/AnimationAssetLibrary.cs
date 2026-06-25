using Spine.Unity;
using UnityEngine;

public class AnimationAssetLibrary : MonoBehaviour
{
    //
    [Header("Single animation")]
    [SerializeField] private AnimationReferenceAsset _dead;
    [SerializeField] private AnimationReferenceAsset _fall;
    [SerializeField] private AnimationReferenceAsset _defense;
    [SerializeField] private AnimationReferenceAsset _buff;
    [SerializeField] private AnimationReferenceAsset _changeForm;

    public AnimationReferenceAsset Dead => _dead;
    public AnimationReferenceAsset Fall => _fall;
    public AnimationReferenceAsset Defense => _defense;
    public AnimationReferenceAsset Buff => _buff;
    public AnimationReferenceAsset ChangeForm => _changeForm;

    [Header("Toggle animation")]
    [SerializeField] private AnimationAssetToggle _idleToggle = new AnimationAssetToggle();
    [SerializeField] private AnimationAssetToggle _moveToggle = new AnimationAssetToggle();

    public AnimationAssetToggle MoveToggle => _moveToggle;
    public AnimationAssetToggle IdleToggle => _idleToggle;

    [Header("Combo animation")]
    [SerializeField] private AnimationAssetCombo _attackCombo = new AnimationAssetCombo();
    [SerializeField] private AnimationAssetCombo _hitCombo = new AnimationAssetCombo();

    public AnimationAssetCombo AttackCombo => _attackCombo;
    public AnimationAssetCombo HitCombo => _hitCombo;
    

    [Header("Progress animation")]
    [SerializeField] private AnimationAssetProgress _skill1Progress = new AnimationAssetProgress();
    [SerializeField] private AnimationAssetProgress _skill2Progress = new AnimationAssetProgress();
    [SerializeField] private AnimationAssetProgress _skill3Progress = new AnimationAssetProgress();
    [SerializeField] private AnimationAssetProgress _skill4Progress = new AnimationAssetProgress();

    public AnimationAssetProgress Skill1Progress => _skill1Progress;
    public AnimationAssetProgress Skill2Progress => _skill2Progress;
    public AnimationAssetProgress Skill3Progress => _skill3Progress;
    public AnimationAssetProgress Skill4Progress => _skill4Progress;
}
