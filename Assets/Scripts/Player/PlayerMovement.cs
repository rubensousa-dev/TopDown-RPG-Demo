using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementInput;
    [SerializeField] private Rigidbody2D rb;

    private Animator animator;

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    private const string lastHorizontal = "LastHorizontal";
    private const string lastVertical = "LastVertical";
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movementInput.Set(InputManager.MoveInput.x, InputManager.MoveInput.y);
        rb.linearVelocity = movementInput * moveSpeed;
        animator.SetFloat(horizontal, movementInput.x);
        animator.SetFloat(vertical, movementInput.y);

        if (movementInput!= Vector2.zero)
        {
            animator.SetFloat(lastHorizontal, movementInput.x);
            animator.SetFloat(lastVertical, movementInput.y);
        }
    }
}
