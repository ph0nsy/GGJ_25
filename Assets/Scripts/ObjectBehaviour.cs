using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectBehaviour : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 targetPosition;
    public float movementDuration = 2f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Behavior")]
    public bool moveOnStart = false;
    public bool loop = false;

    [Header("Visibility Settings")]
    public Camera mainCamera;
    public float checkInterval = 1f;
    public float chanceToToggle = 0.1f; // 10% chance
    public bool quantic = false;
    public Vector3[] nextTargetPosition;

    private Vector3 startPosition;
    private bool isMoving;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (moveOnStart)
        {
            StartMovement();
        }
        Array.Resize(ref nextTargetPosition, nextTargetPosition.Length + 1);
        nextTargetPosition[nextTargetPosition.Length-1]= targetPosition;
    }

    /// <summary>
    /// If the object was already moving, stops the coroutines. Assigns the object's position as the startPosition and starts the MoveCoroutine coroutine.
    /// </summary>
    public void StartMovement()
    {
        if (isMoving)
        {
            StopAllCoroutines();
        }
        startPosition = transform.position;
        StartCoroutine(MoveCoroutine());
    }

    /// <summary>
    /// 
    /// </summary>
    private IEnumerator MoveCoroutine()
    {
        isMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / movementDuration);
            float easedT = easeCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);

            yield return null;
        }

        transform.position = targetPosition;

        if (loop)
        {
            StartMovement();
        }
        else
        {
            isMoving = false;
        }
        if (quantic)
        {
            InvokeRepeating("CheckVisibility", 0f, checkInterval);
        }
    }

    void CheckVisibility()
    {
        StartCoroutine(CheckVisibilityCoroutine());
    }

    private IEnumerator CheckVisibilityCoroutine()
    {
        yield return new WaitForSeconds(checkInterval);
        if (!IsVisibleToCamera())
        {
            if (UnityEngine.Random.value < chanceToToggle)
            {
                gameObject.transform.position = nextTargetPosition[UnityEngine.Random.Range(0, nextTargetPosition.Length)];
            }
        }
    }

    private bool IsVisibleToCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds);
    }
    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }
}