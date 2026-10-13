using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Movement speed variables
    [Header("Movement Speed")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastMoveSpeed = 25f;

    //Move camera around
    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        if (pitch > 180f)
        {
            pitch -= 360f;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    //Movement controls
    private void HandleMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift)
            ? fastMoveSpeed
            : moveSpeed;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = transform.forward;

        Vector3 right = Quaternion.Euler(0f, yaw, 0f)
            * Vector3.right;

        Vector3 movement =
            (forward * vertical) + (right * horizontal);

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        transform.position += movement * speed * Time.deltaTime;
    }

    //Rotate camera around
    private void HandleMouseLook()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    //Disable cursor
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
