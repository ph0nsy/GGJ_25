using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject transitionOut;
    public GameObject transitionOutGameOver;
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    IEnumerator LoadAsyncScene(string sceneName){
        transitionOut.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator LoadAsyncScene(int sceneIdx){
        transitionOutGameOver.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneIdx);
    }

    public void ReloadCurrentScene()
    {
        StartCoroutine(LoadAsyncScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
