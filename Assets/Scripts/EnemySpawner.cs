using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab; // 敵のプレハブ
    [SerializeField] private Transform playerTransform; // プレイヤーのTransform
    [SerializeField] private float spawnInterval = 2f; // 敵の出現間隔
    [SerializeField] private int spawnCount = 5; // 一度に出現する敵の数
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10, 0, 10); // 出現エリアのサイズ
    [SerializeField] private float enemySize = 1f; // 敵の大きさ
    [SerializeField] private float moveSpeed = 2f; // 敵の移動速度

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemies();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // ランダムな位置に敵を出現させる
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                spawnAreaSize.y,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // 敵の大きさを調整
            enemy.transform.localScale = Vector3.one * enemySize;

            // 敵がプレイヤーに向かうように移動するスクリプトを追加
            EnemyMovement enemyMovement = enemy.AddComponent<EnemyMovement>();
            enemyMovement.Initialize(playerTransform, moveSpeed);
        }
    }
}
