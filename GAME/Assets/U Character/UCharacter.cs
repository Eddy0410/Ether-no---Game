using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UCharacter : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    int isWalkingHash;
    int isRunningHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPreesed;
    bool isRunPressed;
    float rotationFactorPerFrame = 15.0f;
    float walkMultiplier = 2.0f;  // Introduce a walk speed multiplier (default to 1.5)
    float runMultiplayer = 4.0f;  // Keep the same run speed multiplier

    float gravity = -9.8f;
    float groundedGravity = -.05f;

    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 1.5f;
    float maxJumpTime = 0.75f;
    bool isJumping = false;
    int isJumpingHash;
    bool isJumpingAnimating = false;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;
        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;

        setupJumpVariables();
    }

    void setupJumpVariables() 
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight / timeToApex);
    }

    void handleJumping() 
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpingAnimating = true;
            currentMovement.y = initialJumpVelocity * .8f;
            currentRunMovement.y = initialJumpVelocity * .8f;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded) 
        {
            isJumping = false;
        
        }
    }

    void OnJump(InputAction.CallbackContext context) 
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkMultiplier;  // Apply walking multiplier
        currentMovement.z = currentMovementInput.y * walkMultiplier;  // Apply walking multiplier

        // Adjust for running (run multiplier should override walk multiplier when running)
        currentRunMovement.x = currentMovementInput.x * runMultiplayer;
        currentRunMovement.z = currentMovementInput.y * runMultiplayer;

        isMovementPreesed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 4.5f;

        if (characterController.isGrounded)
        {
            if (isJumpingAnimating) 
            {
                animator.SetBool(isJumpingHash, false);
                isJumpingAnimating = false;
            }
          
           
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }

        else if (isFalling) 
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }

        else 
        {
           float previousYVelocity = currentMovement.y;
           float newYVelocity = currentRunMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }

        
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPreesed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPreesed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!isMovementPreesed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPreesed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if ((!isMovementPreesed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    void Update()
    {
        HandleRotation();
        handleAnimation();
        

        // Move the character with the appropriate speed based on whether running is active
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }

        handleGravity();
        handleJumping();
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}