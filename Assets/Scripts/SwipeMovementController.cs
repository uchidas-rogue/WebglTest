using UnityEngine;
using DG.Tweening; // 追加

public class SwipeMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 0.05f;
    [SerializeField] float stopSpeed = 5f;
    [SerializeField] float gravity = 9.8f;

    [Header("Ground Settings")]
    [SerializeField] float groundCheckDistance = 0f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector3 groundCheckOffset = Vector3.up * 0.1f;

    Animator harukoAnimator;

    bool isMoving = false;
    Vector3 targetDirection;
    CharacterController characterController;
    float currentSpeed;
    float verticalVelocity;
    bool isGameOver = false;

    [SerializeField] private BarrelSpawner barrelSpawner; // Inspectorでセット
    [SerializeField] private EnemySpawner enemySpawner;   // Inspectorでセット

    [SerializeField] private GameObject gameOverTextBgImg; // GameOverの表示（Inspectorでセット）
    [SerializeField] private GameObject retryButton;   // リトライボタン（Inspectorでセット）
    [SerializeField] private GameObject splitterPlane;   // スプリッターのPlaneオブジェクト（Inspectorでセット）


    private Tween moveTween;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterControllerが見つかりません。コンポーネントを追加してください。");
        }

        Transform childTransform = transform.Find("Haruko");
        if (childTransform != null)
        {
            harukoAnimator = childTransform.GetComponent<Animator>();
            if (harukoAnimator == null)
            {
                Debug.LogError("Animator not found on the child object.");
            }
        }
        else
        {
            Debug.LogError("Child object not found.");
        }
    }

    void Update()
    {
        if (isGameOver) return;
        HandleTouchMove();
    }

    void HandleTouchMove()
    {
        // タッチ入力でX座標の位置に移動
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                Vector3 touchPos = touch.position;
                touchPos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchPos);

                // DOTweenでX座標のみ移動
                MoveToX(worldPos.x);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (harukoAnimator != null)
                    harukoAnimator.SetBool("isWalking", false);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            MoveToX(worldPos.x);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (harukoAnimator != null)
                harukoAnimator.SetBool("isWalking", false);
        }
#endif
    }

    private void MoveToX(float targetX)
    {
        // 既存TweenをKill
        moveTween?.Kill();

        // DOTweenでX座標のみ移動
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        float duration = Mathf.Abs(targetX - transform.position.x) / moveSpeed;
        if (duration < 0.1f) duration = 0.1f;

        moveTween = transform.DOMoveX(targetX, duration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                if (harukoAnimator != null)
                    harukoAnimator.SetBool("isWalking", true);
            })
            .OnComplete(() =>
            {
                if (harukoAnimator != null)
                    harukoAnimator.SetBool("isWalking", false);
            });
    }

    private void OnDisable()
    {
        moveTween?.Kill();
    }
    void ApplyGravity()
    {
        // キャラクターが地面に接しているか確認
        if (characterController.isGrounded)
        {
            verticalVelocity = -groundCheckDistance; // 地面にいるときはY方向の速度を小さく
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; // 重力を適用
        }

        // Y方向の移動を適用して地面にスナップ
        Vector3 gravityMovement = new Vector3(0, verticalVelocity, 0);
        characterController.Move(gravityMovement * Time.deltaTime);
    }
    
    // デバッグ用のギズモ描画
    void OnDrawGizmosSelected()
    {
        // 地面チェックの範囲を可視化
        Gizmos.color = Color.green;
        Vector3 rayStart = transform.position + groundCheckOffset;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * (groundCheckDistance + groundCheckOffset.y));
    }

    private void OnTriggerEnter(Collider other)
    {
        // EnemyまたはBarrelタグのオブジェクトと衝突したらGameOver
        if (!isGameOver && (other.CompareTag("Enemy") || other.CompareTag("Barrel")))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;

        // BarrelSpawnerとEnemySpawnerも停止
        if (barrelSpawner != null)
        {
            barrelSpawner.enabled = false;
        }
        if (enemySpawner != null)
        {
            enemySpawner.enabled = false;
        }

        // splitterPlaneを破棄
        if (splitterPlane != null)
        {
            Destroy(splitterPlane);
        }

        // 全てのEnemyとBarrelの動きを停止
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            // Enemyをdestroyする
            Destroy(enemy);
        }
        foreach (var barrel in GameObject.FindGameObjectsWithTag("Barrel"))
        {
            // Barrelをdestroyする
            Destroy(barrel);
        }

        // GameOverパネル（gameUiCanvas）と文字、リトライボタンを表示
        if (gameOverTextBgImg != null)
            gameOverTextBgImg.SetActive(true);
        if (retryButton != null)
            retryButton.SetActive(true);

        // プレイヤーオブジェクト自体を無効化
        gameObject.SetActive(false);
    }
}
