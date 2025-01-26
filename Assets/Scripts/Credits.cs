using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public RectTransform creditsContent;
    public float scrollDuration = 10f;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        creditsContent.anchoredPosition = startPosition;
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {
        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            creditsContent.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / scrollDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        creditsContent.anchoredPosition = endPosition;

        // Wait for an additional second
        yield return new WaitForSeconds(1f);

        // Load the main menu scene
        this.gameObject.GetComponent<SceneLoader>().LoadScene(mainMenuSceneName);
    }
}