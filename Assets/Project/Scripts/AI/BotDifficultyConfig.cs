using UnityEngine;

[CreateAssetMenu(fileName = "BotDifficultyConfig", menuName = "ScriptableObject/AI/BotDifficultyConfig")]
public class BotDifficultyConfig : ScriptableObject
{
    [Header("Detection Ranges")]
    [Tooltip("Khoảng cách bắt đầu phát hiện Player")]
    public float detectRange = 10f;
    [Tooltip("Khoảng cách tấn công tối ưu (melee)")]
    public float attackRange = 1.5f;
    [Tooltip("Khoảng cách nguy hiểm — Player hitbox vào vùng này → Bot phản ứng")]
    public float threatRange = 2.0f;

    [Header("Timing (giây)")]
    [Tooltip("Delay phản xạ. Easy ≈ 0.8s, Medium ≈ 0.4s, Hard ≈ 0.15s")]
    [Range(0.05f, 1f)]
    public float reactionDelay = 0.4f;
    [Tooltip("Cooldown giữa các lần tấn công")]
    public float attackCooldown = 1.5f;
    [Tooltip("Thời gian giữ Block tối đa")]
    public float blockDuration = 1.0f;
    [Tooltip("Thời gian lùi (Retreat) trước khi tiếp cận lại")]
    public float retreatDuration = 0.6f;

    [Header("Attack Combo")]
    [Tooltip("Thời lượng mỗi animation tấn công (giây). Đo từ Animator của nhân vật.")]
    public float singleAttackDuration = 0.333f;

    [Tooltip("Số đòn combo tối đa bot sẽ thực hiện. Easy=1, Medium=2, Hard=3")]
    [Range(1, 3)]
    public int maxComboHits = 2;

    [Header("Behavior Weights")]
    [Tooltip("Trọng số tấn công khi vào range")]
    [Range(0f, 10f)]
    public float attackWeight = 5f;
    [Tooltip("Trọng số block khi phát hiện threat")]
    [Range(0f, 10f)]
    public float blockWeight = 3f;
    [Tooltip("Trọng số retreat khi phát hiện threat")]
    [Range(0f, 10f)]
    public float retreatWeight = 2f;
    [Tooltip("Trọng số idle — bot 'sai lầm' không phản ứng")]
    [Range(0f, 10f)]
    public float idleWeight = 2f;

    [Header("Fly / Vertical Chase")]
    [Tooltip("Bot ưu tiên bay để tiếp cận khi Player cao hơn X đơn vị")]
    public float flyThreshold = 0.5f;
    [Tooltip("Khoảng cách Y tối đa bot cố bay lên/hạ xuống")]
    public float maxVerticalChase = 5.0f;

    [Header("Pattern Tracking")]
    [Tooltip("Bật/tắt hệ thống nhận dạng lối chơi của Player")]
    public bool enablePatternTracking = true;
    [Tooltip("Độ nhạy thích ứng của Bot (càng cao Bot thích nghi càng mạnh)")]
    [Range(0.1f, 5f)]
    public float patternAdaptationStrength = 1.5f;
    [Tooltip("Thời gian bán rã của cửa sổ EMA (giây) - thời gian để Bot quên lối chơi cũ và thích nghi với lối chơi mới")]
    [Range(0.5f, 10f)]
    public float patternTrackingHalfLife = 3.0f;

    [Header("Preset Description")]
    [TextArea(2, 4)]
    public string description = "";

}
