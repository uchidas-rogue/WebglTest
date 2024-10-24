using UnityEngine;

public class SwipeMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float minSwipeDistance = 50f;
    [SerializeField] float stopSpeed = 5f;
    [SerializeField] float gravity = 9.8f; // 重力を追加

    [Header("Ground Settings")]
    [SerializeField] float groundCheckDistance = 0.2f;
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
    float verticalVelocity; // Y方向の速度を管理

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
            // 子オブジェクトのAnimatorコンポーネントを取得
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

            // 重力を適用し、Y軸のスナップを実施
            ApplyGravity();

            // キャラクターを移動
            characterController.Move(movement);
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
}
