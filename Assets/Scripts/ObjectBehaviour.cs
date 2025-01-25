using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectBehaviour : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 targetPosition;
    public float movementDuration = 2f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Tooltip("The left&uppermost value inside the room")]
    public Transform maxValue;
    [Tooltip("The right&lowermost value inside the room")]
    public Transform minValue;
    [Tooltip("If left empty, it will set 4 points within the range.")]
    public Vector3[] nextTargetPosition;
    private Vector3 startPosition;
    private bool isMoving;
    public Vector3 maxRotation;
    public Vector3 minRotation;
    public float rotationTime = 0.1f;

    [Header("Behavior")]
    public bool moveOnStart = false;
    public bool loop = false;
    // public bool autoUnwrap = false;

    [Header("Visibility Settings")]
    public Camera mainCamera;
    public float checkInterval = 1f;
    public float chanceToToggle = 0.1f; // 10% chance
    public bool quantic = false;

    [Header("Wrapping")]
    public bool unwrapped = false;

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
        if (nextTargetPosition.Length == 0)
        {
            Array.Resize(ref nextTargetPosition, nextTargetPosition.Length + 5);
            SetPoints();
        }
        else
        {
            Array.Resize(ref nextTargetPosition, nextTargetPosition.Length + 1);
        }

        if (!unwrapped)
        {
            InvokeRepeating("Vibrate", 0, rotationTime);
        }

        nextTargetPosition[nextTargetPosition.Length - 1] = targetPosition;
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
        targetPosition = nextTargetPosition[UnityEngine.Random.Range(0, nextTargetPosition.Length)];

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

    public void SetPoints()
    {
        for (int i = 0; i < nextTargetPosition.Length - 2; i++)
        {
            nextTargetPosition[i] = new Vector3 (UnityEngine.Random.Range(minValue.position.x, maxValue.position.x), UnityEngine.Random.Range(minValue.position.y, maxValue.position.y), UnityEngine.Random.Range(minValue.position.z, maxValue.position.z));
        }
    }

    void Vibrate()
    {
        StartCoroutine(VibrateUntilClicked());
    }

    private IEnumerator VibrateUntilClicked()
    {
        if (!unwrapped)
        {
            ControlVibration();
        }
        else
        {
            SendMessage("SpawnVFX");
            BeginMovement();
        }
        yield return null;
    }

    void  BeginMovement()
    {
        StopAllCoroutines();
        StartMovement();
    }

    void ControlVibration()
    {
        Vector3 targetRotation = new Vector3 (UnityEngine.Random.Range(minRotation.x, maxRotation.x), UnityEngine.Random.Range(minRotation.y, maxRotation.y), UnityEngine.Random.Range(minRotation.z, maxRotation.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), rotationTime);
    }
}