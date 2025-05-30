using UnityEngine;
using UnityEngine.UI;
using TMPro; // 追加

public class SwipeMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 0.05f;
    [SerializeField] float minSwipeDistance = 50f;
    [SerializeField] float stopSpeed = 5f;
    [SerializeField] float gravity = 9.8f;

    [Header("Ground Settings")]
    [SerializeField] float groundCheckDistance = 0f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector3 groundCheckOffset = Vector3.up * 0.1f;

    Animator harukoAnimator;
    Vector3 groundCheckPosition = Vector3.zero;
    Vector2 touchStartPos;
    Vector2 touchEndPos;
    bool isSwiping = false;
    bool isMoving = false;
    Vector3 targetDirection;
    CharacterController characterController;
    float currentSpeed;
    RaycastHit groundHit;
    float verticalVelocity;
    bool isGameOver = false;

    [SerializeField] private BarrelSpawner barrelSpawner; // Inspectorでセット
    [SerializeField] private EnemySpawner enemySpawner;   // Inspectorでセット

    [SerializeField] private TextMeshProUGUI gameOverText; // GameOverの文字をTMPに変更
    [SerializeField] private GameObject retryButton;   // リトライボタン（Inspectorでセット）
    [SerializeField] private GameObject splitterPlane;   // スプリッターのPlaneオブジェクト（Inspectorでセット）


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
        HandleSwipeInput();
        MoveCharacter();
    }

    void HandleSwipeInput()
    {
        // タッチ入力の処理
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isSwiping = true;
                    harukoAnimator.SetBool("isWalking", true);
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        touchEndPos = touch.position;
                        Vector2 swipeDelta = touchEndPos - touchStartPos;

                        if (swipeDelta.magnitude > minSwipeDistance)
                        {
                            Vector3 right = Camera.main.transform.right;
                            right.y = 0; // 垂直方向の成分を無視
                            right.Normalize();

                            // X方向のみにターゲット方向を設定
                            targetDirection = right * swipeDelta.x;
                            isMoving = true;
                            currentSpeed = moveSpeed;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    isSwiping = false;
                    harukoAnimator.SetBool("isWalking", false);
                    break;
            }
        }

        // エディタでのデバッグ用マウス入力
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
            harukoAnimator.SetBool("isWalking", true);
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            touchEndPos = Input.mousePosition;
            Vector2 swipeDelta = touchEndPos - touchStartPos;

            if (swipeDelta.magnitude > minSwipeDistance)
            {
                Vector3 right = Camera.main.transform.right;
                right.y = 0; // 垂直方向の成分を無視
                right.Normalize();

                // X方向のみにターゲット方向を設定
                targetDirection = right * swipeDelta.x;
                isMoving = true;
                currentSpeed = moveSpeed;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
            harukoAnimator.SetBool("isWalking", false);
        }
        #endif
    }

    void MoveCharacter()
    {
        if (targetDirection != Vector3.zero && isMoving)
        {
            // スワイプ終了後、徐々に減速
            if (!isSwiping)
            {
                currentSpeed = Mathf.Max(0, currentSpeed - stopSpeed * Time.deltaTime);
                if (currentSpeed <= 0)
                {
                    isMoving = false;
                    targetDirection = Vector3.zero;
                    return;
                }
            }

            // 水平方向 (X方向) のみの移動
            Vector3 movement = targetDirection * currentSpeed * Time.deltaTime;
            movement.z = 0; // Z軸の移動を強制的に0に

            // 斜めの壁にぶつかった際にZ軸方向へ滑らないようにする
            // Move前にX方向だけの移動ベクトルを作成し、壁との衝突でZ方向に押し出されてもX方向のみを維持
            Vector3 beforePosition = transform.position;

            // 重力を適用し、Y軸のスナップを実施
            ApplyGravity();

            // キャラクターを移動
            characterController.Move(movement);

            // 移動後の位置でZ軸方向にずれていたら補正
            Vector3 afterPosition = transform.position;
            if (Mathf.Abs(afterPosition.z - beforePosition.z) > 0.0001f)
            {
                transform.position = new Vector3(afterPosition.x, afterPosition.y, beforePosition.z);
            }
        }
        else
        {
            // 移動していない場合も地面にスナップ
            ApplyGravity();
        }
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
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);
        if (retryButton != null)
            retryButton.SetActive(true);

        // プレイヤーオブジェクト自体を無効化
        gameObject.SetActive(false);
    }
}
