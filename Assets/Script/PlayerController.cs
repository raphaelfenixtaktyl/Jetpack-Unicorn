using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb2D;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] LayerMask wallLayerMask;
    

    float runSpeed = 8f;
    float jumpSpeed = 8f;
    float fallMultiplier = 2.5f;
    float lowJumpMultiplier = 2f;
    float wallSlideSpeed = 4f;

    Vector2 direction, faceRight, faceLeft;

    bool isWallSliding;
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.2f;
    float wallJumpCounter;
    float wallJumpDuration = 0.4f;
    Vector2 wallJumpPower = new Vector2(4f, 8f);
    bool isFacingRight;




    // Start is called before the first frame update
    void Start()
    {
        faceRight = new Vector2(transform.localScale.x, transform.localScale.y);
        faceLeft = new Vector2(-transform.localScale.x, transform.localScale.y);

        rb2D.transform.localScale = faceRight;
    }

    private void FixedUpdate()
    {
        WallSlide();
        WallJump();
        if (isFacingRight)
        {
            transform.localScale = faceRight;
        }
        else
        {
            transform.localScale = faceLeft;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction = Vector2.right;
            rb2D.transform.Translate(direction * runSpeed * Time.fixedDeltaTime);
            isFacingRight = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction = Vector2.left;
            rb2D.transform.Translate(direction * runSpeed * Time.fixedDeltaTime);
            isFacingRight = false;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space)) && isGrounded())
        {
            rb2D.velocity = Vector2.up * jumpSpeed;
        }
        
  
        /*
        if (rb2D.velocity.y < 0)
        {
            rb2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.deltaTime;
        }
        else if(rb2D.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier) * Time.deltaTime;
        }*/
        
    }


    private void WallSlide()
    {
        if (isWalled() && !isGrounded())
        {
            isWallSliding = true;
            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCounter = wallJumpTime;
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) && !isWallJumping && wallJumpCounter > 0)
        {
            isWallJumping = true;
            rb2D.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            Debug.Log("Wall Jump Direction " + wallJumpDirection);
            
            
            //transform.localScale = new Vector2(wallJumpDirection, transform.localScale.y);
        }
    }


    private bool isGrounded()
    {
        RaycastHit2D groundCheck = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, boxCollider.bounds.extents.y + 0.1f, groundLayerMask);
        Color rayColor;
        if(groundCheck.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(boxCollider.bounds.center, Vector2.down * (boxCollider.bounds.extents.y + 0.1f), rayColor);

        return groundCheck.collider != null;

    }


    private bool isWalled()
    {
        return Physics2D.OverlapCircle(boxCollider.bounds.center, (boxCollider.bounds.extents.x + 0.1f), wallLayerMask);
    }


     
}
