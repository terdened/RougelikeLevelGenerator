using UnityEngine;

public class EnemyController : PhysicsObject
{
    public float MaxSpeed = 1f;
    public float HorizontalMovement;
    public bool IsAttacking;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = MaxSpeed * HorizontalMovement;

        bool flipSprite = (spriteRenderer.flipX ? (move.x < -0.01f) : (move.x > 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetFloat("velocityX", Mathf.Abs(velocity.x));
        animator.SetBool("isAttacking", IsAttacking);
        targetVelocity = move * MaxSpeed;
    }
}
