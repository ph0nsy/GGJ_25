using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.VFX;

public class ObjectBehaviour : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 targetPosition;
    public float movementDuration = 2f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Tooltip("The left&uppermost value inside the room")]
    public Transform maxValue; // Required (1)
    [Tooltip("The right&lowermost value inside the room")]
    public Transform minValue; // Required (1)
    [Tooltip("If left empty, it will set 4 points within the range.")]
    public Vector3[] nextTargetPosition; // [Alternative to (1)] populate with 1 Vector3
    private Vector3 startPosition;
    private bool isMoving;
    public Vector3 maxRotation = new Vector3(30, 30, -30);
    public Vector3 minRotation = new Vector3(-30, -30, 30);
    public float rotationTime = 0.1f;

    [Header("Behavior")]
    public bool moveOnStart = false;
    public bool loop = false;
    public bool moves = true;
    [Tooltip("Set the objects that this object triggers")]
    public GameObject[] objectsToMove;
    [ConditionalField("objectsToMove", 0)]
    public Vector3[] positionToMove;

    [Header("Visibility Settings")]
    public Camera mainCamera;
    public float checkInterval = 1f;
    public float chanceToToggle = 0.1f; // 10% chance
    public bool quantic = false;
    public VisualEffectObject vfx_prefab;
    public GameObject endDestroy_prefab;

    [Header("Wrapping")]
    public bool unwrapped = false;

    /// <summary>
    /// Initializes the object's behavior and settings.
    /// </summary>
    private void Start()
    {
        // Initialize camera if not set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Start movement if moveOnStart is true
        if (moveOnStart)
        {
            StartMovement();
        }

        // Set target positions if not provided
        if (nextTargetPosition.Length == 0)
        {
            Array.Resize(ref nextTargetPosition, nextTargetPosition.Length + 5);
            SetPoints();
        }
        else
        {
            Array.Resize(ref nextTargetPosition, nextTargetPosition.Length + 1);
        }

        // Start vibration if not unwrapped
        if (!unwrapped)
        {
            InvokeRepeating("Vibrate", 0, rotationTime);
        }

        // Add current target position to nextTargetPosition array
        nextTargetPosition[nextTargetPosition.Length - 1] = targetPosition;

        // Instantiate endDestroy_prefab
        Instantiate(endDestroy_prefab, this.transform);
    }

    /// <summary>
    /// Starts the movement of the object.
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
    /// Coroutine for smooth movement of the object.
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

    /// <summary>
    /// Initiates the visibility check coroutine.
    /// </summary>
    void CheckVisibility()
    {
        StartCoroutine(CheckVisibilityCoroutine());
    }

    /// <summary>
    /// Coroutine for checking object visibility and potentially changing position.
    /// </summary>
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

    /// <summary>
    /// Checks if the object is visible to the camera.
    /// </summary>
    private bool IsVisibleToCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds);
    }

    /// <summary>
    /// Sets a new target position for the object.
    /// </summary>
    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    /// <summary>
    /// Generates random points within the defined range for nextTargetPosition.
    /// </summary>
    public void SetPoints()
    {
        for (int i = 0; i < nextTargetPosition.Length - 2; i++)
        {
            nextTargetPosition[i] = new Vector3(
                UnityEngine.Random.Range(minValue.position.x, maxValue.position.x),
                UnityEngine.Random.Range(minValue.position.y, maxValue.position.y),
                UnityEngine.Random.Range(minValue.position.z, maxValue.position.z));
        }
    }

    /// <summary>
    /// Initiates the vibration coroutine.
    /// </summary>
    void Vibrate()
    {
        StartCoroutine(VibrateUntilClicked());
    }

    /// <summary>
    /// Coroutine for handling vibration and unwrapping behavior.
    /// </summary>
    private IEnumerator VibrateUntilClicked()
    {
        if (!unwrapped)
        {
            ControlVibration();
        }
        else
        {
            Destroy(Instantiate(vfx_prefab, this.transform), 10.0f);
            yield return new WaitForSeconds(4.0f);
            if (moves) BeginMovement();
            if (objectsToMove.Length > 0)
            {
                for (int i = 0; i < objectsToMove.Length; i++)
                {
                    objectsToMove[i].GetComponent<ObjectBehaviour>().BeginMovement();
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Stops all coroutines and starts the movement.
    /// </summary>
    void BeginMovement()
    {
        StopAllCoroutines();
        StartMovement();
    }

    /// <summary>
    /// Controls the vibration effect by applying random rotation.
    /// </summary>
    void ControlVibration()
    {
        Vector3 targetRotation = new Vector3(
            UnityEngine.Random.Range(minRotation.x, maxRotation.x),
            UnityEngine.Random.Range(minRotation.y, maxRotation.y),
            UnityEngine.Random.Range(minRotation.z, maxRotation.z)
        );
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), rotationTime);
    }

    /// <summary>
    /// Forces the object to lerp to a new position.
    /// </summary>
    public void ForceLerpPosition(Vector3 newVariable)
    {
        isMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / movementDuration);
            float easedT = easeCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPosition, newVariable, easedT);
        }
    }
}
