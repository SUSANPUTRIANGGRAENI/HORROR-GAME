using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovements : MonoBehaviour
{

    public Transform spawnPoint;
    public float respawnDelay = 2f;
    public Transform playerCamera;
    private CharacterController controller;
    private bool isDead = false;

    private float xRotation = 0f;
    private Transform lastEnemyHit;
    private bool isLookingAtEnemy = false;

    private PlayerInput playerInput;
    private InputAction moveAction;

    [SerializeField] private float moveSpeed = 2f;
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

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

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
        if (isLookingAtEnemy)
        {
            return;
        }
        if (isDead) return;

        MovePlayer();
        //FaceCameraForward();
    }

    


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Musuh") && !isDead)
        {
            Debug.Log("Menabrak musuh!");
            // animator.SetTrigger("Dead"); 
            isDead = true;
            lastEnemyHit = hit.transform;
            StartCoroutine(HandleDeathAndRespawn());
        }
    }

    IEnumerator HandleDeathAndRespawn()
    {
        isLookingAtEnemy = true;

        if (playerCamera != null && lastEnemyHit != null)
        {
            Vector3 dirToEnemy = lastEnemyHit.position - transform.position;
            dirToEnemy.y = 0f; // Abaikan tinggi biar tidak miring
            Quaternion bodyLookRotation = Quaternion.LookRotation(dirToEnemy);
            transform.rotation = bodyLookRotation;

            Vector3 camToEnemy = lastEnemyHit.position - playerCamera.position;
            Quaternion camLookRotation = Quaternion.LookRotation(camToEnemy.normalized);
            playerCamera.rotation = camLookRotation;

            xRotation = playerCamera.localEulerAngles.x;
        }

        yield return new WaitForSeconds(respawnDelay);

        controller.enabled = false;
        transform.position = spawnPoint.position;
        controller.enabled = true;

        isDead = false;
        isLookingAtEnemy = false;

        Debug.Log("Respawn selesai");
    }



    void LookAtEnemy()
    {
        if (playerCamera != null && lastEnemyHit != null)
        {
            Vector3 dirToEnemy = lastEnemyHit.position - playerCamera.position;
            Quaternion lookRot = Quaternion.LookRotation(dirToEnemy.normalized);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, lookRot, Time.deltaTime * 5f);
        }
    }

    private void MovePlayer()
    {
        //Vector2 inputDirection = moveAction.ReadValue<Vector2>();
        //Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);

        //if (moveDirection.sqrMagnitude > 0.01f)
        //{
        //    moveDirection = CameraRelativeDirection(moveDirection);
        //    characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        //}

        Vector2 inputDirection = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            moveDirection = CameraRelativeDirection(moveDirection);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }

        UpdateAnimation(inputDirection);

    }

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
