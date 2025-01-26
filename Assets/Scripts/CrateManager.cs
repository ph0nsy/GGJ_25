using System.Collections.Generic;
using UnityEngine;

public class CrateManager : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public int numberOfBoxes = 5;
    public GameObject boxPrefab;
    public float multiplier = 1f;

    private List<GameObject> boxes = new List<GameObject>();

    void Start()
    {
        // Create and position the boxes
        for (int i = 0; i < numberOfBoxes; i++)
        {
            Debug.Log(start.position + ", " + end.position);
            float zPos = Random.Range(start.position.z+1, end.position.z-1);
            GameObject box = Instantiate(boxPrefab, new Vector3 (start.position.x, start.position.y, zPos), Quaternion.identity);
            boxes.Add(box);
        }
    }

    void Update()
    {
        float speed = CalculateSpeed();

        foreach (GameObject box in boxes)
        {
            // Move the box
            box.transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Check if the box has reached the end
            if (box.transform.position.z > end.position.z)
            {
                box.transform.position = start.position;
            }
        }
    }

    float CalculateSpeed()
    {
        float time = Time.time;
        float epsilon = 0.0001f; // Small value to prevent division by zero
        return Mathf.Abs(Mathf.Sin(time) * (0.01f / (Mathf.Cos(time) + epsilon))) * multiplier;
    }
}