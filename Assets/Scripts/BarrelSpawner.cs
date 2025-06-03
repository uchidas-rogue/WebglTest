using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class BarrelSpawner : MonoBehaviour
{
    [SerializeField] private AssetReference barrelPrefabReference; // Addressables用
    [SerializeField] private float spawnInterval = 10f; // 生成間隔（秒）
    [SerializeField] private int spawnCount = 2; // 一度に生成する数
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // 生成位置のオフセット
    [SerializeField] private float randomXRange = 5f; // X軸方向のランダム範囲
    [SerializeField] private BulletSpawner bulletSpawner; // Inspectorからセット
    [SerializeField] private float hpGrowthRate = 1.15f; // hpの増加率（指数の底）をInspectorからセット
    [SerializeField] private float minDistance = 1.5f; // タル同士の最小距離（Inspectorからセット）

    [SerializeField] private AudioClip barrelBurstSound; // タルが破壊されたときの音（Inspectorからセット）
    [SerializeField] private AudioClip levelupSound; // レベルアップの音（Inspectorからセット）

    private float timer = 0f;

    private void Start()
    {
        // 事前ロード
        Addressables.LoadAssetAsync<GameObject>(barrelPrefabReference);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            StartCoroutine(SpawnBarrelsAsync());
            timer = 0f;
        }
    }

    private IEnumerator SpawnBarrelsAsync()
    {
        var types = System.Enum.GetValues(typeof(BarrelBonus.BonusType));
        int typeCount = types.Length;
        Quaternion fixedRotation = Quaternion.Euler(0, 0, 90);

        // 既に使われたX座標を記録
        System.Collections.Generic.HashSet<float> usedX = new System.Collections.Generic.HashSet<float>();

        // 位置を被らせないGetSpawnPos
        Vector3 GetSpawnPos()
        {
            int tryCount = 0;
            while (true)
            {
                float randomX = Random.Range(-randomXRange, randomXRange);
                bool overlap = false;
                foreach (var x in usedX)
                {
                    if (Mathf.Abs(x - randomX) < minDistance)
                    {
                        overlap = true;
                        break;
                    }
                }
                if (!overlap || tryCount > 20)
                {
                    usedX.Add(randomX);
                    return transform.position + spawnOffset + new Vector3(randomX, 0, 0);
                }
                tryCount++;
            }
        }

        IEnumerator SpawnAndInitBarrel(BarrelBonus.BonusType bonusType)
        {
            Vector3 spawnPos = GetSpawnPos();
            var handle = barrelPrefabReference.InstantiateAsync(spawnPos, fixedRotation, transform);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject barrel = handle.Result;
                barrel.transform.localScale *= 2f;

                BarrelBonus bonus = barrel.GetComponent<BarrelBonus>();
                if (bonus != null)
                {
                    bonus.bulletSpawner = bulletSpawner;
                    bonus.barrelBurstSound = barrelBurstSound;
                    bonus.levelupSound = levelupSound;
                    bonus.bonusType = bonusType;

                    int bulletCount = bulletSpawner.BulletCount;
                    int bulletDamage = bulletSpawner.BulletDamage;
                    float randomRate = Random.Range(0.8f, 1.2f);

                    int threshold = 2;
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
                    bonus.UpdateText();
                }
            }
        }

        // 各タイプを1つずつ生成
        for (int t = 0; t < typeCount; t++)
        {
            yield return SpawnAndInitBarrel((BarrelBonus.BonusType)types.GetValue(t));
        }

        // 残りはランダムで生成
        for (int i = typeCount; i < spawnCount; i++)
        {
            int typeCountLocal = System.Enum.GetValues(typeof(BarrelBonus.BonusType)).Length;
            var randomType = (BarrelBonus.BonusType)Random.Range(0, typeCountLocal);

            yield return SpawnAndInitBarrel(randomType);
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