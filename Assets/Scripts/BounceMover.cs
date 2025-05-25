using UnityEngine;
using DG.Tweening;

public class BounceMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private int direction = 1;
    private Tween moveTween;
    private Rigidbody rb;

    private bool isProcessingCollision = false; // 排他制御用フラグ

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // X以外の移動・全回転を固定
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        MoveNext();
    }

    private void MoveNext()
    {
        float distance = 5f;
        Vector3 target = transform.position + Vector3.right * direction * distance;

        moveTween?.Kill();

        // RigidbodyのMovePositionを使ってTween
        moveTween = DOTween.To(
            () => rb.position,
            pos => rb.MovePosition(pos),
            target,
            distance / moveSpeed
        )
        .SetEase(Ease.Linear)
        .OnComplete(() => MoveNext());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessingCollision) return; // すでに処理中なら何もしない

        if (collision.gameObject.CompareTag("Wall"))
        {
            isProcessingCollision = true;
            direction *= -1;
            MoveNext();
            // 少し遅らせてフラグ解除（物理判定の多重呼び出し対策）
            DOVirtual.DelayedCall(0.05f, () => isProcessingCollision = false);
        }
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
    }
}