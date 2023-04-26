using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController2D : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 10f;
    private bool isFacingRight = true;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 12f);

    private bool canDash = true;
    public bool isDashing;
    private float dashingPowerHorizontal = 24f;
    private float dashingPowerVertical = 20f;
    private float dashingTime = 0.15f;
    private float dashingCooldown = 1f;
    private float dashCounter;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform environmentCheck;
    [SerializeField] private LayerMask logLayer;
    [SerializeField] private LayerMask spikesLayer;


    private float fallMultiplier = 10f;
    private float lowJumpMultiplier = 2f;


    //[SerializeField] private GameObject gameOver;
    [SerializeField]private Transform startPostion;

    [SerializeField] private GameObject deathAnim;
    [SerializeField] private Animator deathAnimator;

    private void Awake()
    {
        gameObject.transform.position = startPostion.position;
    }

    private void Update()
    {
        Camera.main.TryGetComponent<CinemachineBrain>(out var brain);
        ///Makes sure the dash is not interupted by other inputs
        if (isDashing)
        {
            return;
        }

        ///Resets Dash when Grounded or Walled
        if(IsGrounded() || IsWalled() || onLog())
        {
            canDash = true;
        }


        ///WASD inputs
        if (!brain.IsBlending)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        

        ///Player Jump
        if (Input.GetButtonDown("Jump") && (IsGrounded() || onLog()))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        ///Gravity Modifier
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        WallJump();
        WallSlide();
        

        ///Dash input
        if (Input.GetKeyDown(KeyCode.RightShift) && canDash)
        {
            canDash = false;
            StartCoroutine(AirDash());
        }


        ///Flips Character local scale while not wall jumping
        if (!isWallJumping)
        {
            Flip();
        }

        
    }

    private void FixedUpdate()
    {

        ///Makes sure the dash is not interupted by other inputs
        if (isDashing)
        {
            return;
        }
        
        ///Player Run
        if (!isWallJumping && !isDashing && !onLog())
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

       
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private bool onLog()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, logLayer);
    }

    private bool OnSpikes()
    {
        return Physics2D.OverlapCircle(environmentCheck.position, 0.6f, spikesLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            Debug.Log("Wall Sliding");
            Debug.Log("Velocity Y " + rb.velocity.y);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator AirDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (horizontal == 0f && (vertical > 0f || vertical < 0f))
        {
            //rb.velocity = new Vector2(0f, vertical * dashingPowerVertical);
            rb.AddForce(new Vector2(0f, vertical * dashingPowerVertical), ForceMode2D.Impulse);
            //transform.Translate(new Vector3(0f, vertical * dashingPowerVertical, 0f));
        }
        else if (vertical == 0f && (horizontal > 0f || horizontal < 0f))
        {
            //rb.velocity = new Vector2(horizontal * dashingPowerHorizontal, 0f);
            rb.AddForce(new Vector2(horizontal * dashingPowerHorizontal, 0f), ForceMode2D.Impulse);
        }
        else if ((horizontal > 0f || horizontal < 0f) && (vertical > 0f || vertical < 0f))
        {
            //rb.velocity = new Vector2((horizontal * dashingPowerHorizontal) / 2f, (vertical * dashingPowerVertical) / 2f );
            rb.AddForce(rb.velocity = new Vector2((horizontal * dashingPowerHorizontal) / 2f, (vertical * dashingPowerVertical) / 2f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        Invoke(nameof(StopAirDashing), 0f);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


    private void StopAirDashing()
    {
        isDashing = false;
        rb.velocity *= 0.2f * Time.fixedDeltaTime;
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (OnSpikes())
        {
            deathAnim.SetActive(true);
            deathAnim.transform.parent = null;
            deathAnim.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);
            deathAnimator.Play("item-feedback-Animation");

            if (!deathAnimator.GetCurrentAnimatorStateInfo(0).IsName("item-feedback-Animation"))
            {
                Invoke(nameof(ResartLevel), 0.5f);
            }
            //gameOver.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            startPostion.position = collision.gameObject.transform.position;
        }
    }

    public void ResartLevel()
    {
        gameObject.transform.position = startPostion.position;
        gameObject.SetActive(true);
        deathAnim.SetActive(false);
        //gameOver.SetActive(false);

        isDashing = false;
        isWallJumping = false;
        isWallSliding = false;

        rb.gravityScale = 1f;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
        Gizmos.DrawWireSphere(environmentCheck.position, 0.5f);
    }
}
