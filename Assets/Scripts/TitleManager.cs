using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    public RectTransform imageRectTransform;
    public float duration = 2f;
    public float waitTime = 0f;
    public float minSize = 0.1f;
    public float midSize = 1.2f;
    public float maxSize = 1f;

    private void Start()
    {
        // Ensure the image starts at scale 0.1
        imageRectTransform.localScale = Vector3.one * minSize;

        // Start the scaling coroutine
        StartCoroutine(ScaleImage());
    }

    private IEnumerator ScaleImage()
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.one * minSize;
        Vector3 midScale = Vector3.one * midSize;
        Vector3 endScale = Vector3.one * maxSize;

        yield return new WaitForSeconds(waitTime);

        // First half: scale from 0.1 to 1.2
        while (elapsedTime < duration / 2)
        {
            imageRectTransform.localScale = Vector3.Lerp(startScale, midScale, elapsedTime / (duration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset elapsed time for the second half
        elapsedTime = 0f;

        // Second half: scale from 1.2 to 1
        while (elapsedTime < duration / 2)
        {
            imageRectTransform.localScale = Vector3.Lerp(midScale, endScale, elapsedTime / (duration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end exactly at scale
        imageRectTransform.localScale = endScale;
    }
}
