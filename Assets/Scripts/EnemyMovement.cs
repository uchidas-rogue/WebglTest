using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform playerTransform;
    private float moveSpeed;
    private bool isMoving = true; // 敵が移動中かどうかのフラグ

    [Header("Destruction Settings")]
    [SerializeField] private int requiredHits = 1; // 破壊されるまでの接触回数
    private int currentHits = 0; // 現在の接触回数

    public void Initialize(Transform player, float speed)
    {
        playerTransform = player;
        moveSpeed = speed;
    }

    private void Update()
    {
        if (playerTransform != null && isMoving)
        {
            // プレイヤーの方向を向く
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーまたは停止したenemy_cubeに接触した場合、移動を停止
        if (other.CompareTag("Player") || (other.CompareTag("Enemy") && !other.GetComponent<EnemyMovement>().isMoving))
        {
            StopMovement();
        }

        // Bulletタグのオブジェクトとの接触処理
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("Bulletに接触しました。現在のヒット数: " + currentHits);
            currentHits++; // 接触回数をカウント

            if (currentHits >= requiredHits)
            {
                 Debug.Log("指定回数に達したため、enemy_cubeを破壊します。");
                Destroy(gameObject); // 規定回数以上接触したら破壊
            }
        }
    }

    private void StopMovement()
    {
        isMoving = false;
        moveSpeed = 0; // 移動速度を0にすることで停止を確実にする
    }
}
