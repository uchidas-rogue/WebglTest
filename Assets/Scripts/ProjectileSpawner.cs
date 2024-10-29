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
        for (int i = 0; i < projectileCount; i++)
        {
            // キャラクターの前方向に対する発射位置
            Vector3 spawnPosition = transform.position + transform.forward + transform.right * ((i - (projectileCount - 1) / 2f) * sideSpacing);

            // Projectileのインスタンスを生成
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            
            // キャラクターの前方向に基づくターゲット位置を設定
            Vector3 targetPosition = spawnPosition + transform.forward * maxDistance;

            // DOTweenを使用して、キャラクターの前方向に球体を移動
            projectile.transform.DOMove(targetPosition, maxDistance / projectileSpeed)
                .SetEase(Ease.Linear)
                .OnKill(() =>
                {
                    Destroy(projectile); // 移動終了後にオブジェクトを破棄
                });
        }
    }
}
