using UnityEngine;
using DG.Tweening;

public class BarrelMover : MonoBehaviour
{
    [SerializeField] private float moveDistance = 40f;      // 進む距離
    [SerializeField] private float moveSpeed = 4f;          // 移動速度
    [SerializeField] private float rollSpeed = 360f;        // 1秒あたりの回転角度（度）

    private Tween moveTween;
    private Tween rotateTween;

    private void Start()
    {
        // Barrelの親オブジェクトを取得
        Transform parent = transform.parent != null ? transform.parent : transform;

        Vector3 target = parent.position + Vector3.back * moveDistance; // Z軸マイナス方向へ
        float duration = moveDistance / moveSpeed;

        // DOTweenで親オブジェクトを移動
        moveTween = parent.DOMove(target, duration)
            .SetEase(Ease.Linear);

        // Barrel自身の回転（X軸方向に回転）
        float totalAngle = -rollSpeed * duration;
        rotateTween = transform.DORotate(
            new Vector3(transform.eulerAngles.x + totalAngle, transform.eulerAngles.y, transform.eulerAngles.z),
            duration,
            RotateMode.FastBeyond360
        ).SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
        rotateTween?.Kill();
    }
}