using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform playerTransform;
    private float moveSpeed;
    private bool isMoving = true; // 敵が移動中かどうかのフラグ

    private int hp = 1; // HPとして扱う

    public void Initialize(Transform player, float speed, int hpValue)
    {
        playerTransform = player;
        moveSpeed = speed;
        hp = hpValue;
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
            // Bullet.csから減らす値を取得
            Bullet bulletScript = other.GetComponent<Bullet>();
            int damage = 1;
            if (bulletScript != null)
            {
                // Bullet.csにpublic int damage = 1; などのメンバを用意してください
                damage = bulletScript.damage;
            }

            Debug.Log($"Bulletに接触しました。現在のHP: {hp} ダメージ: {damage}");
            hp -= damage; // Bullet.csから取得した値でHPを減る

            if (hp <= 0)
            {
                Debug.Log("HPが0になったため、enemy_cubeを破壊します。");
                Destroy(gameObject); // HPが0以下なら破壊
            }
        }
    }

    private void StopMovement()
    {
        isMoving = false;
        moveSpeed = 0; // 移動速度を0にすることで停止を確実にする
    }
}
