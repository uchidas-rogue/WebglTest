using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float swipeThreshold = 50f; // フリックのしきい値
    private Animator animator;

    private Vector2 startTouchPosition, endTouchPosition;
    private float swipeDistance;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // フリック入力の検出
        DetectSwipe();

        // 移動量の計算
        Vector3 moveDirection = new Vector3(endTouchPosition.x - startTouchPosition.x, 0, endTouchPosition.y - startTouchPosition.y);
        moveDirection.Normalize(); // ベクトルを正規化して移動量を一定にする

        // 移動しきい値をチェック
        if (moveDirection.magnitude > swipeThreshold / 100f) // しきい値を調整
        {
            Debug.Log($"Moving: {moveDirection}"); // 移動方向を表示
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // マウスボタンを押したとき
        {
            startTouchPosition = Input.mousePosition; // マウスの位置を取得
        }
        else if (Input.GetMouseButtonUp(0)) // マウスボタンを離したとき
        {
            endTouchPosition = Input.mousePosition;
        }
    }

}
