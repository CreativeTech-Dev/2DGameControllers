using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    public float speed = 7f;
    Vector2 move;
    Rigidbody2D rb;
    public float jumpForce = 5f;
    int jumpsLeft;
    public int maxJumps = 2;
    bool grounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        move.x = Input.GetAxis("Horizontal") * speed;
        move.y = rb.velocity.y;

        rb.velocity = move;

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0) {
            jumpsLeft -= 1;
            move.y = jumpForce;
            rb.velocity = move;

        }

        if (grounded) {
            jumpsLeft = maxJumps;
        }
        

    }


}
