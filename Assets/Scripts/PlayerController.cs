using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Animator wateringCanAnimator;
    public float moveSpeed = 5.0f; // Walking speed
    public float gravity = -9.81f; // Gravity force
    public float jumpHeight = 1.5f; // Jump height

    private CharacterController characterController;
    private Vector3 velocity; // Current velocity for gravity and jumping



    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        MovePlayer();

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            wateringCanAnimator.SetBool("IsWatering", true);
            GameManager.Instance.player.IsWatering = true;

        }
        else if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            wateringCanAnimator.SetBool("IsWatering", false);
            GameManager.Instance.player.IsWatering = false;
        }

        GameManager.Instance.player.UseWateringCan();
    }

    private void MovePlayer()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical"); // W/S
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Apply movement
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Apply gravity
        if(characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply jumping
        if(Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply vertical velocity
        characterController.Move(velocity * Time.deltaTime);
    }
}
