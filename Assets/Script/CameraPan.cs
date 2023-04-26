using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraPan : MonoBehaviour
{
    public GameObject virtualCam;


    private float originalTImeScale;
    private void Start()
    {
        originalTImeScale = Time.timeScale;
    }

    private void Update()
    {
        Camera.main.TryGetComponent<CinemachineBrain>(out var brain);

        if (brain.IsBlending)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = originalTImeScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            virtualCam.SetActive(false);
        }
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
}
