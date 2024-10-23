using UnityEngine;

public class Player : MonoBehaviour
{
public float moveSpeed = 5f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 moveDirection = new Vector3(0, 0, move);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (move != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        transform.Rotate(0, turn * 100 * Time.deltaTime, 0);
    }
}
