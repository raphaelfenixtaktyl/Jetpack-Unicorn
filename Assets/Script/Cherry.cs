using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cherry : MonoBehaviour
{
    [SerializeField] private LayerMask cherryLayer;
    //[SerializeField] private TextMeshProUGUI cherryCounter;
    private int cherries;
    // Update is called once per frame
    void Update()
    {
        cherries = PlayerPrefs.GetInt("Cherries");
        //cherryCounter.text = "X " + cherries;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Cherry"))
        {
            collision.gameObject.SetActive(false);
            PlayerPrefs.SetInt("Cherries", PlayerPrefs.GetInt("Cherries") + 1);
        }
    }
}
