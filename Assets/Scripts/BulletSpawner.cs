using UnityEngine;
using DG.Tweening;

public class BulletSpawner : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab; // 発射する弾のプレハブ
    [SerializeField] private float bulletSpeed = 10f; // 発射速度
    [SerializeField] private float spawnInterval = 1f; // 発射間隔（秒）
    [SerializeField] private float maxDistance = 10f; // 弾が移動して破棄される最大距離
    [SerializeField] private float sideSpacing = 0.5f; // 側方にずれる距離
    [SerializeField] private float launchDelay = 0.1f; // 弾の発射タイミングをずらす間隔
    [SerializeField] private int bulletDamage = 1; // 弾のダメージ
    [SerializeField] private int bulletCount = 1; // 一度に発射する弾の数
    [SerializeField] private AudioSource bulletSound; // 発射音（Inspectorからセット）

    private float spawnTimer; // 発射間隔の計測用タイマー
    private Sequence launchSequence; // DOTweenのシーケンス

    private void Update()
    {
        // タイマーが発射間隔を超えたら発射
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnBullets();
            spawnTimer = 0f;
        }
    }

    private void SpawnBullets()
    {
        // DOTweenのシーケンスを作成してタイミングを制御
        launchSequence = DOTween.Sequence();

        for (int i = 0; i < bulletCount; i++)
        {
            int index = i; // ローカルスコープの変数を使用

            // シーケンスに遅延と発射処理を追加
            launchSequence.AppendCallback(() =>
            {
                Vector3 spawnPosition = transform.position + transform.forward + transform.right * ((index - (bulletCount - 1) / 2f) * sideSpacing);
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

                // Bullet.csのsetterでパラメータをセット
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.damage = bulletDamage; // bulletDamageで上書き
                    bulletScript.SetBulletParams(bulletSpeed, maxDistance, transform.forward);
                }
                bulletSound?.Play(); // 発射音を再生
            });

            // 次の弾の発射を少し遅らせる
            launchSequence.AppendInterval(launchDelay);
        }
    }

    public void IncreaseBulletCount(int value)
    {
        bulletCount += value;
    }

    public void IncreaseBulletDamage(int value)
    {
        bulletDamage += value;
    }

    public int BulletCount => bulletCount;

    public int BulletDamage => bulletDamage;

    private void OnDisable()
    {
        // シーケンスが存在する場合は停止
        if (launchSequence != null)
        {
            launchSequence.Kill();
        }
    }
}
