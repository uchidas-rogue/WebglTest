using UnityEngine;
using DG.Tweening; // DOTweenの名前空間を追加

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // 発射する球のプレハブ
    [SerializeField] private float projectileSpeed = 10f; // 発射速度
    [SerializeField] private int projectileCount = 1; // 一度に発射する球の数
    [SerializeField] private float spawnInterval = 1f; // 発射間隔（秒）
    [SerializeField] private float projectileLifetime = 2f; // 球体の生存時間

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
        // キャラクターの前方向（forward）に球を発射
        for (int i = 0; i < projectileCount; i++)
        {
            // Projectileのインスタンスを生成
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            
            // DOTweenで移動処理を設定
            Vector3 targetPosition = projectile.transform.position + transform.forward * projectileSpeed;
            projectile.transform.DOMove(targetPosition, projectileLifetime).SetEase(Ease.Linear).OnKill(() =>
            {
                Destroy(projectile); // 生存時間が過ぎたらオブジェクトを破棄
            });
        }
    }
}
