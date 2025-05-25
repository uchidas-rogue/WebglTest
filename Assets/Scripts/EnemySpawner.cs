using UnityEngine;
using Model;
using VContainer;

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
    [SerializeField] private int enemyHp = 1; // 敵の初期HP

    [Inject] public Score scoreModel { get; set; } // VContainerでInject

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
            // スポナーの位置を基準にエリア内でランダムに出現
            Vector3 center = transform.position;
            Vector3 spawnPosition = new Vector3(
                center.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                center.y + spawnAreaSize.y,
                center.z + Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            // z軸で逆向きに回転
            Quaternion rotation = Quaternion.Euler(0, 180f, 0);

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, rotation);

            enemy.transform.localScale = Vector3.one * enemySize;

            // スコアに応じてHPを増加（例：scoreが100増えるごとに+1、最低3）
            int dynamicHp = Mathf.Max(3, enemyHp + (scoreModel.scoreRP.Value / 100));

            EnemyMovement enemyMovement = enemy.AddComponent<EnemyMovement>();
            enemyMovement.Initialize(playerTransform, moveSpeed, dynamicHp); // HPを渡す
            enemyMovement.scoreModel = scoreModel; // InjectしたscoreModelを渡す
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // エリアをワイヤーフレームで表示
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, spawnAreaSize.y / 2, 0), spawnAreaSize);
    }
#endif
}
