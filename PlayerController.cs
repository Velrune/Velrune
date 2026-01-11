using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [Header("Attachable object")]
    [SerializeField] private CharacterController cc;
    [SerializeField] private Camera p_Camera;
    [SerializeField] private Animator p_Animator;

    [Space(20)]
    [Header("Attachable Scripts")]
    [SerializeField] private StaminaSystem ss;

    [Space(20)]
    [Header("Player settings")]
    [Tooltip("Sensitivity for player's mouse")]
    [SerializeField] private float Sensitivity = 2.0f;
    [Tooltip("Speed control for player")]
    [SerializeField] private float walkSpeed = 3.0f;
    [Tooltip("Speed control for player")]
    [SerializeField] private float runSpeed = 7.0f;
    [Tooltip("Speed control for player")]
    [SerializeField] private float crouchSpeed = 2.0f;
    [Tooltip("Gravity settings")]
    [SerializeField] private float gravity = 7.0f;

    private float Xoffset = 60.0f;

    private float rotationY;
    private float mouseX;
    private float mouseY;
    private float currentSpeed;
    private float crouchBlend;

    private InputSystem_Actions inputActions;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private bool crouchKey;
    private bool isMoving;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += _ => Jump();

        inputActions.Player.Sprint.performed += _ => { if (!ss.isTired && isMoving) ss.ChangeRunningState(true); };
        inputActions.Player.Sprint.canceled += _ => ss.ChangeRunningState(false);

        inputActions.Player.Crouch.performed += _ => crouchKey = true;
        inputActions.Player.Crouch.canceled += _ => crouchKey = false;

        inputActions.Player.ToggleCrouch.performed += _ => crouchKey = !crouchKey;
    }

    void OnEnable() { inputActions.Enable(); }
    void OnDisable() { inputActions.Disable(); }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRunning();
        HandleInput();
        Move();
        ApplyGravity();
        Crouch();
        mouseX = inputActions.Player.MouseX.ReadValue<float>() * Sensitivity * Time.deltaTime;
        mouseY = inputActions.Player.MouseY.ReadValue<float>() * Sensitivity * Time.deltaTime;
    }

    void LateUpdate()
    {
        RotateCamera();
    }

    void Move()
    {
        cc.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (cc.isGrounded && moveDirection.y < 0)
            moveDirection.y = -2f;
        else
            moveDirection.y -= gravity * Time.deltaTime;
    }

    void RotateCamera()
    {
        transform.Rotate(Vector3.up * mouseX);

        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, -Xoffset, Xoffset);
        p_Camera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
    }

    private void Jump()
    {
        throw new NotImplementedException();
    }

    void Crouch()
    {
        float target;
        if (crouchKey)
            target = 1;
        else
            target = 0;

        crouchBlend = Mathf.MoveTowards(crouchBlend, target, Time.deltaTime * crouchSpeed);
        p_Animator.SetFloat("crouchBlend", crouchBlend);
    }

    void HandleInput()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
    }

    void HandleRunning()
    {
        isMoving = moveInput.sqrMagnitude > 0.01f;

        if (!isMoving) {
            ss.ChangeRunningState(false);
            return;
        }

        if (ss.isRunning)
        {
            currentSpeed = runSpeed;
            ss.SpendStamina();
        }
        else
            currentSpeed = walkSpeed;
    }
}
