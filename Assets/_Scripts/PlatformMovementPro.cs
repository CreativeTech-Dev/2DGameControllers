using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovementPro : MonoBehaviour
{

    public float speed = 7f;
    Vector2 move;
    Rigidbody2D rb;

    public float resetYValue = -10f;
    private Vector3 startPos;

    [Header("Jumping")]
    public float jumpPower = 5f;
    public float jumpTime;
    public float jumpMultiplier;
    public float fallMultiplier;
    bool isJumping;
    float jumpCounter;
    Vector2 vecGravity;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;
    float jumpBufferCounter;

    [Header("Ground")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    Vector2 capsuleCheckSize = new Vector2(1f, 0.2f);

    [Header("Dash")]
    public bool canDash = true;
    private bool isDashing;
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    TrailRenderer tr;
    bool isFacingRight = true;



    // Start is called before the first frame update
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();

        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        }
        else {
            coyoteTimeCounter -= Time.deltaTime;
        }

        move.x = Input.GetAxis("Horizontal") * speed;
        move.y = rb.velocity.y;

        if (!isDashing) {
            rb.velocity = move;
        }

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        }
        else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) {
            isJumping = true;
            jumpCounter = 0;
            jumpBufferCounter = 0;
            move.y = jumpPower;
            rb.velocity = move;
        }

        if (rb.velocity.y > 0 && isJumping) {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime) {
                isJumping = false;
            }

            float t = jumpCounter / jumpTime;
            float currentJumpMultiplier = jumpMultiplier;

            if (t > 0.5f) {
                currentJumpMultiplier = jumpMultiplier * (1 - t);
            }

            rb.velocity += vecGravity * (currentJumpMultiplier * Time.deltaTime);
        }

        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
            jumpCounter = 0;
            coyoteTimeCounter = 0;
            if (rb.velocity.y > 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
        }

        if (rb.velocity.y < 0) {
            rb.velocity -= vecGravity * (fallMultiplier * Time.deltaTime);

        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) {
            StartCoroutine(Dash());
        }

        Flip();
    }

    void FixedUpdate() {
        if (transform.position.y < resetYValue) {
            rb.velocity = Vector2.zero;
            transform.position = startPos;
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCapsule(groundCheck.position, capsuleCheckSize, CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    void Flip() {
        if ((isFacingRight && move.x < 0 ) || (!isFacingRight && move.x > 0)){
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    IEnumerator Dash() {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

}
