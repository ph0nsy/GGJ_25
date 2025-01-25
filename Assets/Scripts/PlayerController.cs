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
    [Range(1.0f,10.0f)]
    float mouseSensitivity = 0;
    float yRotation = 0;
    [HideInInspector]
    bool ownsBubbles = false;
    [SerializeField]
    [Range(5,100)]
    private int currBubbles = 20;
    private Vector3 moveVector;
    private float currJumpHeight = 0;
    private bool hasBubbleWrap = true;
    [SerializeField]
    [Range(5.0f,10.0f)]
    private float burstForce = 5.0f;
    [Range(1.0f,5.0f)]
    public float popCooldown = 5.0f;
    private float currPopCD = 0.0f;
    private bool onBurst = false;

    private GameObject wrappedObjectList;
    private CharacterController controller;
    private Material bubbleWrap;
    private Camera playerCamera;


     void Awake()
     {
         QualitySettings.vSyncCount = 0;
         Application.targetFrameRate = 60;
     }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //bubbleWrap = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material; // Where the first child is the canvas, the next is the pannel that anchors the object and the next is the gO
        playerCamera = Camera.main;
        wrappedObjectList = GameObject.Find("Wrapped Objects");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Interact();
        PopBubble();
    }

    void Move() {
        // Player Movement
        moveVector = (transform.forward.normalized*Input.GetAxis("Vertical")) + (transform.right.normalized*Input.GetAxis("Horizontal"));
        // Pop Burst
        if (onBurst) moveVector = new Vector3(this.transform.forward.x * -burstForce, 0, this.transform.forward.z * -burstForce)/speed;
        if(onBurst && (currJumpHeight < -(burstForce/speed) || controller.isGrounded)) onBurst = false;
        // Jump
        if (Input.GetAxis("Jump") > 0 && controller.isGrounded) currJumpHeight = jumpForce/speed;
        if (!controller.isGrounded) currJumpHeight -= gravity*Time.deltaTime/speed; 
        moveVector.y = currJumpHeight;
        // Move Player
        controller.Move(moveVector * Time.deltaTime * speed);

        // Rotation
        // Vertical Rotation
        yRotation += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(yRotation, -60, 40), 0, 0);
        // Horizontal Rotation
        this.transform.eulerAngles = new Vector3(0, Input.GetAxis("Mouse X")*mouseSensitivity, 0) + this.transform.eulerAngles;
        playerCamera.transform.rotation = Quaternion.Euler(new Vector3(playerCamera.transform.eulerAngles.x, this.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z));
    }

    void Interact() {
        // 

        if(Input.GetKeyDown("e")){
            Transform closest = null;
            
            // Package Interaction
            foreach(Transform child in GameObject.Find("Interactable List").transform.GetChild(0).transform){ // Interactable List es una lista con todos los objetos con los que puede interactuar el jugador
                // Packages
                if((Vector3.Distance(child.transform.position, this.transform.position) < maxInteractionRange) && (closest == null || Vector3.Distance(child.transform.position, this.transform.position) < Vector3.Distance(closest.transform.position, this.transform.position))) closest = child;
            }

            // Get Wrapping Papper
            if(closest != null) {
                if (closest.name == "BubbleWrap") { hasBubbleWrap = true; closest.gameObject.SetActive(false); }
            }
        }
    }

    void PopBubble()
    {
        if(hasBubbleWrap) {
            if(Input.GetMouseButtonDown(0)) { 
                if(wrappedObjectList != null && wrappedObjectList.transform.childCount > 0) {
                    foreach(Transform wrappedObject in wrappedObjectList.transform){
                        // Changing comparison value sets distance you can interact with the object (recomended never more than character size x2)
                        if(Math.Abs(Vector3.Dot((this.transform.position - wrappedObject.position).normalized, this.transform.forward)) < 0.5f) { UnwrapObject(); break; }
                    }
                }
                if(currPopCD < 0.1f) { MovementBurst(); currPopCD = popCooldown; } 
            }

            if(currPopCD >= 0.1f) currPopCD -= Time.deltaTime; 
        }
    }

    void MovementBurst()
    {
        Debug.Log("Burst");
        currJumpHeight = playerCamera.transform.forward.y*-burstForce/speed;
        onBurst = true;
    }

    void UnwrapObject() 
    {
        // Remove object from wrappedObjectsList and into freeObjectsList
        // Set boolean on other obj
    }
}
