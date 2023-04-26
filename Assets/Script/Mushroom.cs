using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{

    //[SerializeField]private Transform startPosition;
    private float mushroomLaunchTimer;
    [SerializeField]private float mushroomLaunchPower = 30f;
    [SerializeField] private CapsuleCollider2D circleCollider;

    [SerializeField] private PlayerController2D player;
    private Transform playerPosition;
    private Rigidbody2D playerRB;
    [SerializeField] private bool isDashing;

    // Start is called before the first frame update
    void Start()
    {
        //startPosition = new Vector2(boxCollider.bounds.center.x + 1, (boxCollider.bounds.center.y + boxCollider.bounds.extents.y) + 2f);
        //Debug.Log("START POSITION MUSHROOM :: " + startPosition);

        playerPosition = player.GetComponent<Transform>();
        playerRB = player.GetComponent<Rigidbody2D>();
        isDashing = player.GetComponent<PlayerController2D>().isDashing;
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(MushroomLaunch());
        }
    }

    */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            playerRB.AddForce(new Vector2(0f, 1 * mushroomLaunchPower), ForceMode2D.Impulse);

            if (isDashing)
            {
                Debug.Log("IS DASHING");
                mushroomLaunchTimer = 0.3f;
            }
            else
            {
                mushroomLaunchTimer = 0.2f;
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y * 0.4f);
            }

            //Invoke(nameof(StopLaunch), mushroomLaunchTimer);
            //StartCoroutine(MushroomLaunch());
        }
    }
    


    private void StopLaunch()
    {
        playerRB.velocity *= 0.2f * Time.deltaTime;
    }

}
