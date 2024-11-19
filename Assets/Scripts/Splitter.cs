using UnityEngine;

public class Splitter : MonoBehaviour
{
    [Header("Split Settings")]
    [SerializeField] private GameObject splitPrefab;       // 分裂後のオブジェクトのプレハブ
    [SerializeField] private int splitCount = 3;           // 一度の分裂で生成されるオブジェクトの数
    [SerializeField] private float positionOffset = 0.1f;  // 生成位置のわずかなずれ
    [SerializeField] private int maxSplitTimes = 3;        // 最大分裂回数

    private int currentSplitCount = 0;                     // 現在の分裂回数

    private void OnCollisionEnter(Collision collision)
    {
        // Bulletタグを持つオブジェクトと接触し、最大分裂回数に達していない場合に分裂
        if (collision.gameObject.CompareTag("Bullet") && currentSplitCount < maxSplitTimes)
        {
            Rigidbody bulletRb = collision.gameObject.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                Split(bulletRb);
                currentSplitCount++;
            }
            else
            {
                Debug.LogWarning("接触したBulletオブジェクトにRigidbodyがありません。");
            }
        }
    }

    private void Split(Rigidbody sourceRb)
    {
        // 分裂オブジェクトの生成
        for (int i = 0; i < splitCount; i++)
        {
            // 生成位置を少しずらして分裂オブジェクトを配置
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-positionOffset, positionOffset),
                Random.Range(-positionOffset, positionOffset),
                Random.Range(-positionOffset, positionOffset)
            );

            // 分裂オブジェクトを生成
            GameObject splitObject = Instantiate(splitPrefab, spawnPosition, Quaternion.identity);

            // 分裂オブジェクトのRigidbodyを取得し、速度と方向を元のBulletのRigidbodyから引き継ぐ
            Rigidbody splitRb = splitObject.GetComponent<Rigidbody>();
            if (splitRb != null)
            {
                splitRb.linearVelocity = sourceRb.linearVelocity; // Bulletオブジェクトの速度を継承
            }

            // 分裂オブジェクトにもSplitterコンポーネントを追加し、設定を引き継ぐ
            Splitter splitScript = splitObject.GetComponent<Splitter>();
            if (splitScript != null)
            {
                splitScript.currentSplitCount = currentSplitCount; // 現在の分裂回数を引き継ぐ
                splitScript.maxSplitTimes = maxSplitTimes;         // 最大分裂回数も設定
            }
        }
    }
}
