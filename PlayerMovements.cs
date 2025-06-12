using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundMask;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value);
    }

    private Animator animator;
    private CharacterController characterController;
    private Transform camTransform;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");

        if (moveAction == null)
        {
            Debug.LogError("Move action not found! Check your Input Actions settings.");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found!");
        }

        camTransform = Camera.main.transform;
    }

    private void Update()
    {
        MovePlayer();
        FaceCameraForward();  // Selalu menghadap ke depan kamera, statis
    }

    private void MovePlayer()
    {
        Vector2 inputDirection = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            moveDirection = CameraRelativeDirection(moveDirection);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        UpdateAnimation(inputDirection);
    }

    // Gerak relatif terhadap kamera
    private Vector3 CameraRelativeDirection(Vector3 inputDirection)
    {
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return (forward * inputDirection.z + right * inputDirection.x).normalized;
    }

    // Fungsi untuk membuat karakter selalu menghadap depan kamera (statik)
    private void FaceCameraForward()
    {
        Vector3 forward = camTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        if (forward.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 10f);
        }
    }

    private void UpdateAnimation(Vector2 input)
    {
        if (animator == null) return;

        float threshold = 0.1f;

        bool isDiagonal = Mathf.Abs(input.x) > threshold && Mathf.Abs(input.y) > threshold;

        animator.SetBool("isMovingForward", input.y > threshold);
        animator.SetBool("isMovingBackward", input.y < -threshold);
        animator.SetBool("isMovingRight", input.x > threshold);
        animator.SetBool("isMovingLeft", input.x < -threshold);
        animator.SetBool("isMovingDiagonally", isDiagonal);
    }

    public void SetSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
    }
}