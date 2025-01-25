using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    [Range(1.0f,100.0f)]
    float speed = 0;
    [SerializeField]
    [Range(1.0f,100.0f)]
    float jumpForce = 0;
    [SerializeField]
    [Range(1.0f,100.0f)]
    float gravity = 0;
    [SerializeField]
    [Range(1.0f,100.0f)]
    float maxInteractionRange = 0;
    [SerializeField]
    [Range(1.0f,5.0f)]
    float mouseSensitivity = 0;
    float yRotation = 0;
    [HideInInspector]
    bool ownsBubbles = false;
    [SerializeField]
    [Range(5,100)]
    private int currBubbles = 20;
    private Vector3 moveVector;
    private float currJumpHeight = 0;
    
    private CharacterController controller;
    private Material bubbleWrap;
    private Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //bubbleWrap = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material; // Where the first child is the canvas, the next is the pannel that anchors the object and the next is the gO
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Interact();
    }

    void Move() {
        // Player Movement
        moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetAxis("Jump") > 0 && controller.isGrounded) currJumpHeight = jumpForce/speed; 
        if (!controller.isGrounded) currJumpHeight -= gravity*Time.deltaTime/speed;
        moveVector = new Vector3(Input.GetAxis("Horizontal"), currJumpHeight, Input.GetAxis("Vertical"));
        controller.Move(moveVector * Time.deltaTime * speed);

        // Camera
        yRotation += (-Input.GetAxis("Mouse Y") * mouseSensitivity);
        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(yRotation, -45, 40), 0, 0);
        this.transform.Rotate(new Vector3(0, -Input.GetAxis("Mouse X")*mouseSensitivity, 0));
    }

    void Interact() {
        if(Input.GetKeyDown("e")){
            Transform closest = null;
            foreach(Transform child in GameObject.Find("Interactable List").transform){ // Interactable List es una lista con todos los objetos con los que puede interactuar el jugador
                if((Vector3.Distance(child.transform.position, this.transform.position) < maxInteractionRange) && (closest == null || Vector3.Distance(child.transform.position, this.transform.position) < Vector3.Distance(closest.transform.position, this.transform.position))) closest = child;
            }
        }
        //if(Input.GetKeyDown("mouse 0") && ownsBubbles) { bubbleWrap.SetFloat("_SrcBlend", (float)(currBubbles/100)); --currBubbles; }
    }
}
