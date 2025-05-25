using UnityEngine;
using DG.Tweening;

public class BounceMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private int direction = 1;
    private Tween moveTween;
    private Rigidbody rb;

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
        if (collision.gameObject.CompareTag("Wall"))
        {
            direction *= -1;
            MoveNext();
        }
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
    }
}