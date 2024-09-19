using UnityEngine;

public class playerFPS : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    public float gravity = 20f; // Gravity for falling

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        if (characterController.isGrounded)
        {
            // We are grounded, so reset the Y movement.
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        }
        else
        {
            // If not grounded, continue applying gravity to Y direction.
            moveDirection.x = forward.x * curSpeedX + right.x * curSpeedY;
            moveDirection.z = forward.z * curSpeedX + right.z * curSpeedY;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the character controller
        characterController.Move(moveDirection * Time.deltaTime);
        #endregion

        #region Handles Rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion
    }
}
