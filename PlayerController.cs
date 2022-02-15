using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform viewPoint;
    [SerializeField] float mouseSens = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;
    [SerializeField] float moveSpeed = 5f, runSpeed = 8f;
    private Vector3 moveDir, movement;
    private float activeMoveSpeed;
    public bool isRunning;
    public bool isIdle;

    [SerializeField] CharacterController charCon;
    private Camera cam;
    public float jumpForce = 10f, gravityMod = 2.5f;
    public Transform groundCheckPoint; //using this as a bool instead of character controller is grounded to eliminate isgrounded bug
    public bool isGrounded;
    public bool standingOnEdge;
    public LayerMask groundLayers, playerLayer;
    private float yTimer, yThen, yNow;
    private float yTimeStep = 0.5f;
    [SerializeField] float interactRange = 5f;
    private GameObject interactedObject;
    //[SerializeField] Camera cameraDirection;
    
    







    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        isIdle = true;
        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            GetMouseInput();
            MovePlayer();
            OnEdgeCalculator();
            PauseMenu();
            InteractWithObjects();
        }
    }

    void GetMouseInput()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSens;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, 
            transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -80f, 80f);

        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,
            viewPoint.rotation.eulerAngles.z);
    }

    void MovePlayer()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift) && moveDir.z > 0)
        {
            isRunning = true;
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
            isRunning = false;
        }
        float yVel = movement.y; //take current y
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel; //apply current y to itself so that falling power can constantly increase each frame with the next line
        if (charCon.isGrounded) { movement.y = 0; } //Reset Physics.gravity applied if grounded
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);
        if (Input.GetButtonDown("Jump") && isGrounded) //normal jump
        {
            movement.y = jumpForce;
        }


        if (Input.GetButtonDown("Jump") && standingOnEdge) //edge jump
        {
            movement.y = jumpForce;
        }


        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod; //Apply gravity to current y
        charCon.Move(movement * Time.deltaTime);

        if (movement.x == 0 && movement.z == 0) isIdle = true; else isIdle = false;
    }

    

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            if (MatchManager.instance.state == MatchManager.GameState.Playing)
            {
                cam.transform.position = viewPoint.position;
                cam.transform.rotation = viewPoint.rotation;
            }
            else
            {
                cam.transform.position = MatchManager.instance.mapCamPoint.position;
                cam.transform.rotation = MatchManager.instance.mapCamPoint.rotation;
            }
            
        }
    }

    private void OnEdgeCalculator()
    {
        float yThen = transform.position.y;
        yTimer += Time.deltaTime;
        if (yTimer >= yTimeStep)
        {
            yNow = transform.position.y;
            yTimer = 0;
            
        }
        
        if (yThen == yNow && !isGrounded)
        {
            standingOnEdge = true;
        }
        else standingOnEdge = false;

    }

    void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        } 
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void InteractWithObjects()
    {


        if (Input.GetKeyDown(KeyCode.E))
        {
            
            
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, interactRange, playerLayer))
            {
                if (hit.transform.gameObject.tag == "Interactable")
                    interactedObject = hit.transform.gameObject;
                else return;

            }
            var nearestGameObject = interactedObject;
            if (nearestGameObject == null) return;
            var interactable = nearestGameObject.GetComponent<IInteractable>();
            if (interactable == null) return;
            interactable.Interact();


            interactedObject = null;

           
        }

    }



}
