using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour {
    [Header("References")] 
    public CharacterController controller;
    public Transform camHolderTransform;
    public Transform camTransform;
    public Camera cam;
    
    [Header("Settings")] 
    public float mouseSensitivity;

    [Header("Properties")]
    public float moveSpeed;
    public float jumpPower;
    public float gravityAmount;
    public float attackCooldown;
    public bool canMove = true;
    public bool canLook = true;
    
    [Header("Ground Check Settings")]
    public Vector3 groundCheckOrigin;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    
    [Header("Head Properties")]
    public float headbobSpeed;
    public float headbobIntensity;
    public float headrotIntensity;

    // movement variables
    private Vector2 moveInput;
    private Vector3 move;
    private bool isMoving => moveInput.magnitude > 0f && canMove;
    private bool isRunning => isMoving && Input.GetKey(KeyCode.LeftShift);
    
    // jump variables
    private float lastGroundTime;
    
    // gravity variables
    private Vector3 velocity;
    private bool onGround => Physics.CheckSphere(transform.position + groundCheckOrigin, groundCheckRadius, groundLayer);

    // mouse look variables
    private Vector2 mouseInput;
    private float xRot;
    
    // headbob variables
    private float headbobSinValue;
    
    // headrot variables
    private float zRot;
    
    // weapon variables
    private int attackIndex;
    private float attackRate;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        HandleMouseLook();
        HandleStepOffset();
        HandleMovement();
        HandleRunning();
        HandleJump();
        HandleGravity();
        HandleHeadbob();
        HandleHeadrot();
    }

    void HandleMouseLook() {
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * (mouseSensitivity * Time.deltaTime);
        
        if (!canLook) return;

        xRot -= mouseInput.y;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.Rotate(Vector3.up * mouseInput.x);
        camHolderTransform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    void HandleStepOffset() {
        controller.stepOffset = onGround ? .7f : 0f;
    }

    void HandleMovement() {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        move = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (!canMove) return;

        controller.Move(move * (moveSpeed * (isRunning ? 1.5f : 1f) * Time.deltaTime));
    }

    void HandleRunning() {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, isRunning ? 75f : 60f, 3f * Time.deltaTime);
        camTransform.localEulerAngles = new Vector3(Mathf.Lerp(camTransform.localEulerAngles.x, isRunning ? 10f : 0f, 5f * Time.deltaTime), camTransform.localEulerAngles.y, camTransform.localEulerAngles.z);
    }

    void HandleJump() {
        if (onGround)
            lastGroundTime = Time.time;
        
        if (Input.GetKeyDown(KeyCode.Space) && lastGroundTime + .075f >= Time.time)
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravityAmount);
    }

    void HandleGravity() {
        velocity.y += gravityAmount * Time.deltaTime;

        if (onGround && velocity.y < 0f)
            velocity.y = -2f;
        
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleHeadbob() {
        if (isMoving)
            headbobSinValue += headbobSpeed * (isRunning ? 2f : 1f) * Time.deltaTime;

        float camY = Mathf.Lerp(camTransform.localPosition.y, isMoving ? Mathf.Sin(headbobSinValue) * headbobIntensity : 0f, headbobSpeed * (isRunning ? 2f : 1f) * Time.deltaTime);
        camTransform.localPosition = new Vector3(0f, camY, 0f);
    }

    void HandleHeadrot() {
        zRot = Mathf.Lerp(zRot, isMoving ? -moveInput.x * headrotIntensity : 0f, 5f * Time.deltaTime);
        camHolderTransform.eulerAngles = new Vector3(camHolderTransform.eulerAngles.x, camHolderTransform.eulerAngles.y, zRot);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOrigin, groundCheckRadius);
    }
}