using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    private float bulletSpeed = 10f;
    private float maxDistance = 10f;
    private Vector3 direction = Vector3.forward;
    public bool isSplitGenerated = false;
    public int damage = 1;

    private Tween moveTween; // Tweenを保持

    public void SetBulletParams(float speed, float distance, Vector3 dir)
    {
        bulletSpeed = speed;
        maxDistance = distance;
        direction = dir;
    }

    private void Start()
    {
        // DOTweenで移動
        Vector3 targetPosition = transform.position + direction.normalized * maxDistance;
        moveTween = transform.DOMove(targetPosition, maxDistance / bulletSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (this != null && gameObject != null)
                {
                    Destroy(gameObject);
                }
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Barrel"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時にTweenも破棄
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }

    // bulletSpeed, maxDistanceのgetter
    public float GetBulletSpeed() => bulletSpeed;
    public float GetMaxDistance() => maxDistance;
    public Vector3 GetDirection() => direction;
}