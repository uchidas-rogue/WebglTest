using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    [SerializeField] private GameObject barrelPrefab; // Barrelのプレハブ
    [SerializeField] private float spawnInterval = 10f; // 生成間隔（秒）
    [SerializeField] private int spawnCount = 1; // 一度に生成する数
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // 生成位置のオフセット
    [SerializeField] private float randomXRange = 5f; // X軸方向のランダム範囲
    [SerializeField] private BulletSpawner bulletSpawner; // Inspectorからセット
    [SerializeField] private float hpGrowthRate = 1.15f; // hpの増加率（指数の底）をInspectorからセット

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBarrels();
            timer = 0f;
        }
    }

    private void SpawnBarrels()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            float randomX = Random.Range(-randomXRange, randomXRange);
            Vector3 spawnPos = transform.position + spawnOffset + new Vector3(randomX, 0, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, 90); // Z軸に90度回転
            GameObject barrel = Instantiate(barrelPrefab, spawnPos, rotation);
            barrel.transform.localScale *= 2f; // 大きさを2倍に

            // BarrelBonusがアタッチされていればBulletSpawnerとhpとbonusTypeをセット
            BarrelBonus bonus = barrel.GetComponent<BarrelBonus>();
            if (bonus != null)
            {
                bonus.bulletSpawner = bulletSpawner;

                // bonusTypeをランダムでセット
                int typeCount = System.Enum.GetValues(typeof(BarrelBonus.BonusType)).Length;
                bonus.bonusType = (BarrelBonus.BonusType)Random.Range(0, typeCount);

                int bulletCount = bulletSpawner.BulletCount;
                int bulletDamage = bulletSpawner.BulletDamage;
                float randomRate = Random.Range(0.8f, 1.5f); // 0.8～1.5倍のランダム倍率

                int threshold = 2; // ここで閾値を設定
                int hp;
                if (bulletCount + bulletDamage <= threshold)
                {
                    hp = 20;
                }
                else
                {
                    hp = Mathf.Max(20, Mathf.RoundToInt(20f * Mathf.Pow(hpGrowthRate, bulletCount + bulletDamage)));
                }

                bonus.hp = Mathf.RoundToInt(hp * randomRate);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // X軸方向の生成範囲を可視化
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + spawnOffset;
        Vector3 size = new Vector3(randomXRange * 2, 1f, 1f);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}