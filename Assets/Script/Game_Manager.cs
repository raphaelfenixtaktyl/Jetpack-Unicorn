using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private GameObject loadScreen;
    [SerializeField] private Image loadingFillBar;
   
    public void QuitGame()
    {
        Application.Quit();
    }


    public void LoadingScreen(int sceneID)
    {
       StartCoroutine(LoadSceneAsynch(sceneID));
    }

    IEnumerator LoadSceneAsynch(int sceneID)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        loadScreen.SetActive(true);
        loadingFillBar.fillAmount = 0f;

        while (!operation.isDone)
        {
            float progressBar = Mathf.Clamp01(operation.progress / 0.9f);

            loadingFillBar.fillAmount = progressBar;
            yield return null;
        }
       
    }
}
