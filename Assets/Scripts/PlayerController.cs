using System;
using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [Tooltip("Movement speed of the character (except upwards)")]
    [SerializeField]
    [Range(1.0f,100.0f)]
    float speed = 0;
    [Tooltip("Jump force of the character (decreasing until jump's apex)")]
    [SerializeField]
    [Range(1.0f,100.0f)]
    float jumpForce = 0;
    [Tooltip("Gravity for the character (factor by which character's y position is decreased)")]
    [SerializeField]
    [Range(1.0f,100.0f)]
    float gravity = 0;
    private Vector3 moveVector;
    private float currJumpHeight = 0;
    [Header("Interaction")]
    [Tooltip("Maximum range that the player can interact with any object (recomended never more than character size times 2)")]
    [SerializeField]
    [Range(0.5f,5.0f)]
    float maxInteractionRange = 0.75f;
    [Tooltip("Mouse Sensitivity, both vertically and horizontally")]
    [SerializeField]
    [Range(1.0f,10.0f)]
    float mouseSensitivity = 0;
    float yRotation = 0;
    [Header("Pop Bubbles")]
    [HideInInspector]
    public bool ownsBubbles = false;
    [Tooltip("Maximum number of bubbles (unused)")]
    [SerializeField]
    [Range(5,100)]
    private int currBubbles = 20;
    [Tooltip("Only set true at start on Edit, not on final build")]
    [SerializeField]
    private bool hasBubbleWrap = false;
    [Tooltip("Force with which the player is pushed backwads when poping a bubble")]
    [SerializeField]
    [Range(5.0f,10.0f)]
    private float burstForce = 5.0f;
    private bool onBurstInteract = false;
    [Tooltip("How many seconds must pass until the player can pop another bubble")]
    [Range(1.0f,5.0f)]
    public float popCooldown = 5.0f;
    private float currPopCD = 0.0f;
    private bool onBurst = false;

    private GameObject wrappedObjectList;
    private CharacterController controller;
    private Material bubbleWrap;
    private Camera playerCamera;
    private GameObject interactObjectList;
    private GameObject freeObjectList;
    private GameObject[] allObjects;
    [Header("Animation")]
    public Animator handAnimator;


     void Awake()
     {
         QualitySettings.vSyncCount = 0;
         Application.targetFrameRate = 60;
     }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!controller) controller = GetComponent<CharacterController>();
        //bubbleWrap = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material; // Where the first child is the canvas, the next is the pannel that anchors the object and the next is the gO
        if(!playerCamera) playerCamera = Camera.main;
        if(!wrappedObjectList) wrappedObjectList = GameObject.Find("WrappedObjects");
        if(!interactObjectList) interactObjectList = GameObject.Find("InteractableObjects");
        if(!freeObjectList) freeObjectList = GameObject.Find("FreeObjects");
        if(allObjects == null) allObjects = GameObject.FindGameObjectsWithTag("Object");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        CheckNearest();
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
        playerCamera.transform.eulerAngles = new Vector3(Mathf.Clamp(yRotation, -88, 40), 0, 0);
        // Horizontal Rotation
        this.transform.eulerAngles = new Vector3(0, Input.GetAxis("Mouse X")*mouseSensitivity, 0) + this.transform.eulerAngles;
        playerCamera.transform.rotation = Quaternion.Euler(new Vector3(playerCamera.transform.eulerAngles.x, this.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z));
    }

    void CheckNearest()
    {
        GameObject closest = null;
        foreach(GameObject obj in allObjects) {
            float isLookin = Vector3.Dot((obj.transform.position - transform.position).normalized, this.transform.forward);
            float currDist =  Vector3.Distance(obj.transform.position, this.transform.position) - (controller.radius*2);
            if(!closest && isLookin > 0.75f && currDist < maxInteractionRange) closest = obj;
            else if(closest && isLookin > 0.75f && currDist < Vector3.Distance(this.transform.position, closest.transform.position)) closest = obj;
        }
        if(closest != null) {
            Debug.Log("Closest: " + closest.name);

            if (closest.transform.parent.name == interactObjectList.name) { this.transform.GetChild(1).GetChild(0).gameObject.SetActive(true); this.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); }
            if (closest.transform.parent.name == wrappedObjectList.name) { this.transform.GetChild(1).GetChild(1).gameObject.SetActive(true); this.transform.GetChild(1).GetChild(0).gameObject.SetActive(false); }
        }
        else {
            this.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
    }

    void Interact() {

        if(Input.GetKeyDown("e")){
            Transform closest = null;
            
            // Package Interaction
            foreach(Transform child in interactObjectList.transform){
                // Packages
                if((Vector3.Distance(child.transform.position, this.transform.position) - (controller.radius*2) < maxInteractionRange) && (closest == null || Vector3.Distance(child.transform.position, this.transform.position) < Vector3.Distance(closest.transform.position, this.transform.position))) closest = child;
            }

            // Get Wrapping Papper
            if(closest != null) {
                if (closest.name == "BubbleWrap") { hasBubbleWrap = true; closest.gameObject.SetActive(false); }
                else Debug.Log(closest.name); // Activate Object Behaviour (Interactable)
            }
            
        }
    }

    void PopBubble()
    {
        if(hasBubbleWrap) {
            if(Input.GetMouseButtonDown(0)) { 
                handAnimator.SetBool("Popin", true);
                StartCoroutine(PopLogic());
            }

            if(currPopCD >= 0.1f) currPopCD -= Time.deltaTime; 
        }
    }

    IEnumerator PopLogic(){
        yield return new WaitForSeconds(0.5f);
        if(wrappedObjectList != null && wrappedObjectList.transform.childCount > 0) {
            foreach(Transform wrappedObject in wrappedObjectList.transform){
                float isLookin = Vector3.Dot((wrappedObject.transform.position - transform.position).normalized, this.transform.forward);
                if(isLookin > 0.75  && (Vector3.Distance(wrappedObject.transform.position, this.transform.position) - (controller.radius*2) < maxInteractionRange)) { UnwrapObject(wrappedObject); onBurstInteract = true; break; }
            }
        }
        if(currPopCD < 0.1f && !onBurstInteract) { MovementBurst(); currPopCD = popCooldown; } 
        onBurstInteract = false;
        handAnimator.SetBool("Popin", false);
    }

    void MovementBurst()
    {
        // Modify Satisfaction Bar
        currJumpHeight = playerCamera.transform.forward.y*-burstForce/speed;
        onBurst = true;
    }

    void UnwrapObject(Transform other) 
    {
        Debug.Log(other.name);
        // Play animation
        other.GetComponent<ObjectBehaviour>().unwrapped = true;
        other.SetParent(freeObjectList.transform,true);
    }
}
