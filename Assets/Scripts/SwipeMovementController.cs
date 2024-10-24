using UnityEngine;

public class SwipeMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float stopSpeed = 5f;

    [Header("Ground Settings")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheckOffset = Vector3.up * 0.1f;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;
    private bool isMoving = false;
    private Vector3 targetDirection;
    private CharacterController characterController;
    private float currentSpeed;
    private RaycastHit groundHit;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterControllerが見つかりません。コンポーネントを追加してください。");
        }
    }

    private void Update()
    {
        HandleSwipeInput();
        MoveCharacter();
        SnapToGround();
    }

    private void HandleSwipeInput()
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
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        touchEndPos = touch.position;
                        Vector2 swipeDelta = touchEndPos - touchStartPos;

                        if (swipeDelta.magnitude > minSwipeDistance)
                        {
                            Vector3 forward = Camera.main.transform.forward;
                            Vector3 right = Camera.main.transform.right;
                            forward.y = 0;
                            right.y = 0;
                            forward.Normalize();
                            right.Normalize();

                            targetDirection = forward * swipeDelta.y + right * swipeDelta.x;
                            targetDirection.Normalize();
                            isMoving = true;
                            currentSpeed = moveSpeed;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    isSwiping = false;
                    break;
            }
        }

        // エディタでのデバッグ用マウス入力
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            touchEndPos = Input.mousePosition;
            Vector2 swipeDelta = touchEndPos - touchStartPos;

            if (swipeDelta.magnitude > minSwipeDistance)
            {
                Vector3 forward = Camera.main.transform.forward;
                Vector3 right = Camera.main.transform.right;
                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();

                targetDirection = forward * swipeDelta.y + right * swipeDelta.x;
                targetDirection.Normalize();
                isMoving = true;
                currentSpeed = moveSpeed;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }
        #endif
    }

    private void MoveCharacter()
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

            // キャラクターの回転
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 水平方向の移動のみを適用
            Vector3 movement = targetDirection * currentSpeed * Time.deltaTime;
            movement.y = 0; // Y軸の移動を強制的に0に
            characterController.Move(movement);
        }
    }

    private void SnapToGround()
    {
        // キャラクターの足元から地面までのレイキャスト
        Vector3 rayStart = transform.position + groundCheckOffset;
        if (Physics.Raycast(rayStart, Vector3.down, out groundHit, groundCheckDistance + groundCheckOffset.y, groundLayer))
        {
            // 現在位置から地面までの距離を計算
            float distanceToGround = groundHit.distance - groundCheckOffset.y;
            
            // 地面との高さの差がある場合、スナップ
            if (distanceToGround > 0)
            {
                Vector3 snapMovement = Vector3.down * distanceToGround;
                characterController.Move(snapMovement);
            }
        }
    }

    // デバッグ用のギズモ描画
    private void OnDrawGizmosSelected()
    {
        // 地面チェックの範囲を可視化
        Gizmos.color = Color.green;
        Vector3 rayStart = transform.position + groundCheckOffset;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * (groundCheckDistance + groundCheckOffset.y));
    }
}