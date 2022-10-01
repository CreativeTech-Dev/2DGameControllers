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
    public Vector2 capsuleCheckSize = new Vector2(0.8f, 0.2f);
    [Header("Dash")]
    public bool canDash = true;
    private bool isDashing;
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    [Header("Wall slide system")]
    public bool useWallJunping = true;
    public Transform wallCheckPos;
    public Vector2 wallCapsuleCheckSize = new Vector2(0.1f, 1.5f);
    public bool isGrounded;
    public bool isWallTouch;
    public bool isSliding;
    public float wallSlidingSpeed = 1f;
    public float wallJumpDuration;
    public Vector2 wallJumpFore;
    bool wallJumping;


    TrailRenderer tr;
    bool isFacingRight = true;



    // Start is called before the first frame update
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        isWallTouch = false;

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

        if (useWallJunping) {

        
            isWallTouch = Physics2D.OverlapCapsule(wallCheckPos.position, wallCapsuleCheckSize, CapsuleDirection2D.Vertical, 0, groundLayer);
            if (isWallTouch && !isGrounded && move.x != 0) {
                isSliding = true;
            } else {
                isSliding = false;
            }

            if (isSliding) {
                move.y = Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue);//-wallSlidingSpeed;
                rb.velocity = move;//new Vector2(rb.velocity.x, wallSlidingSpeed);//Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }
            if (wallJumping) {
                move.x *= -wallJumpFore.x;
                move.y = wallJumpFore.y;
                rb.velocity = move;
            }
        }
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
        else if (isSliding && Input.GetButtonDown("Jump")) {
            wallJumping = true;
            Invoke("StopWallJump", wallJumpDuration);
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

        if (Input.GetButtonUp("Jump") && !isWallTouch) {
            isJumping = false;
            jumpCounter = 0;
            coyoteTimeCounter = 0;
            if (rb.velocity.y > 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
        }

        if (rb.velocity.y < 0 && !isWallTouch) {
            rb.velocity -= vecGravity * (fallMultiplier * Time.deltaTime);

        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) {
            StartCoroutine(Dash());
        }

        Flip();
    }

    public Vector3 GetRestartPos() {
        return GetComponent<PlayerReset>().GetRestartPos();
    }

    void StopWallJump() {
        wallJumping = false;
    }

    void FixedUpdate() {
        if (transform.position.y < resetYValue) {
            rb.velocity = Vector2.zero;
            transform.position = GetRestartPos();
        }
    }

    bool IsGrounded() {
        isGrounded = Physics2D.OverlapCapsule(groundCheck.position, capsuleCheckSize, CapsuleDirection2D.Horizontal, 0, groundLayer);
        return isGrounded;
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
