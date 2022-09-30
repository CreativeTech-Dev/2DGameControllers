using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovementAdv : MonoBehaviour
{

    public float speed = 7f;
    Vector2 move;
    Rigidbody2D rb;

    [Header("Jumping")]
    public float jumpPower = 5f;
    public int maxJumps = 2;
    public float jumpTime;
    public float jumpMultiplier;
    public float fallMultiplier;
    int jumpsLeft;
    bool isJumping;
    float jumpCounter;
    Vector2 vecGravity;

    [Header("Ground")]
    bool grounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    Vector2 capsuleCheckSize = new Vector2(1f, 0.2f);


    // Start is called before the first frame update
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.OverlapCapsule(groundCheck.position, capsuleCheckSize, CapsuleDirection2D.Horizontal, 0, groundLayer);
        if (grounded && !isJumping) {
            jumpsLeft = maxJumps;
        }

        move.x = Input.GetAxis("Horizontal") * speed;
        move.y = rb.velocity.y;

        rb.velocity = move;

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0) {
            jumpsLeft -= 1;
            grounded = false;
            isJumping = true;
            jumpCounter = 0;
            move.y = jumpPower;
            rb.velocity = move;
        }
        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
            jumpCounter = 0;

            if (rb.velocity.y > 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
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

        if (rb.velocity.y < 0) {
            rb.velocity -= vecGravity * (fallMultiplier * Time.deltaTime);

        }

        

    }


}
