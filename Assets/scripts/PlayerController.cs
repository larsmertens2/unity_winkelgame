using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;

    // Voeg deze variabelen toe bovenaan de klasse
    public float jumpForce = 5f;
    private bool isGrounded;

    private Rigidbody rb;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Make the Rigidbody kinematic if the player is not the owner.
        rb.isKinematic = !IsOwner;  // If not the owner, make it kinematic (i.e., no physics simulation)

        // Disable camera for non-local players
        if (!IsOwner && cameraTransform != null)
        {
            cameraTransform.gameObject.SetActive(false);
        }

        // Lock the cursor for the owner (local player)
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (!IsOwner) return;  // Only the owner controls movement and camera

        LookAround();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;  // Only the owner controls movement

        Move();
    }

    // Voeg deze methode toe om te controleren of de speler op de grond staat
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Move()
    {
        // Getting inputs for movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculating the desired movement direction
        Vector3 moveDir = transform.right * x + transform.forward * z;
        Vector3 targetVelocity = moveDir * moveSpeed;

        // Setting the velocity change for the Rigidbody
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0;

        // Applying force to the Rigidbody for movement
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void LookAround()
    {
        // Getting mouse input for looking around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Updating camera rotation based on mouse input
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
}