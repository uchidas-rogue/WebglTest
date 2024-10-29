using UnityEngine;
using DG.Tweening;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // 発射する球のプレハブ
    [SerializeField] private float projectileSpeed = 10f; // 発射速度
    [SerializeField] private int projectileCount = 1; // 一度に発射する球の数
    [SerializeField] private float spawnInterval = 1f; // 発射間隔（秒）
    [SerializeField] private float maxDistance = 10f; // 球体が移動して破棄される最大距離
    [SerializeField] private float sideSpacing = 0.5f; // 側方にずれる距離
    [SerializeField] private float launchDelay = 0.1f; // 球の発射タイミングをずらす間隔

    private float spawnTimer; // 発射間隔の計測用タイマー

    private void Update()
    {
        // タイマーが発射間隔を超えたら発射
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnProjectiles();
            spawnTimer = 0f;
        }
    }

    private void SpawnProjectiles()
    {
        // DOTweenのシーケンスを作成してタイミングを制御
        Sequence launchSequence = DOTween.Sequence();

        for (int i = 0; i < projectileCount; i++)
        {
            int index = i; // ローカルスコープの変数を使用

            // シーケンスに遅延と発射処理を追加
            launchSequence.AppendCallback(() =>
            {
                Vector3 spawnPosition = transform.position + transform.forward + transform.right * ((index - (projectileCount - 1) / 2f) * sideSpacing);

                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

                Vector3 targetPosition = spawnPosition + transform.forward * maxDistance;

                // DOTweenで球体を移動
                projectile.transform.DOMove(targetPosition, maxDistance / projectileSpeed)
                    .SetEase(Ease.Linear)
                    .OnKill(() =>
                    {
                        Destroy(projectile);
                    });
            });

            // 次の球の発射を少し遅らせる
            launchSequence.AppendInterval(launchDelay);
        }
    }
}
