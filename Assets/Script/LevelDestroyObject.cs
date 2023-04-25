using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDestroyObject : MonoBehaviour
{
    [SerializeField] private GameObject winnerScreen;
    [SerializeField] private PlayerController2D player;
    bool isDashing;
    private void Update()
    {
        isDashing = player.isDashing;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective") && isDashing)
        {
            collision.gameObject.SetActive(false);
            winnerScreen.SetActive(true);
        }

        if (collision.gameObject.CompareTag("Destructable") && isDashing)
        {
            collision.gameObject.SetActive(false);
        }
    }
}
