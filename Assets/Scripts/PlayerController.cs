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

        if(GameManager.Instance.menuHistory.Count != 0) return;
        if(Input.GetKeyDown(KeyCode.Mouse1) && GameManager.Instance.player.AvailableWater > 0)
        {
            wateringCanAnimator.SetBool("IsWatering", true);
            GameManager.Instance.player.IsWatering = true;
        }
        else if(Input.GetKeyUp(KeyCode.Mouse1) || GameManager.Instance.player.AvailableWater <= 0 && GameManager.Instance.player.IsWatering)
        {
            wateringCanAnimator.SetBool("IsWatering", false);
            GameManager.Instance.player.IsWatering = false;
        }

        GameManager.Instance.player.UseWateringCan();
    }

    private void MovePlayer()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if(GameManager.Instance.menuHistory.Count != 0) horizontal = vertical = 0;
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Apply horizontal movement
        Vector3 movement = move * moveSpeed * Time.deltaTime;

        // Gravity & jumping
        if(characterController.isGrounded)
        {
            if(velocity.y < 0)
                velocity.y = -2f; // Keep grounded

            if(Input.GetButtonDown("Jump") && GameManager.Instance.menuHistory.Count != 0)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            // Only add gravity when not grounded
            velocity.y += gravity * Time.deltaTime;
        }

        // Combine horizontal movement with vertical velocity
        movement.y = velocity.y * Time.deltaTime;
        characterController.Move(movement);
    }

}
