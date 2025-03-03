using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Singleton

    private static PlayerController instance;
    public static PlayerController Instance
    {
        get { return instance; }
        set
        {
            if(instance != null)
            {
                Destroy(value.gameObject);
                return;
            }
            instance = value;
        }
    }

    #endregion

    public Animator wateringCanAnimator;
    public float moveSpeed = 5.0f; // Walking speed
    public float gravity = -9.81f; // Gravity force
    public float jumpHeight = 1.5f; // Jump height
    public GameObject wateringCan;
    public GameObject wateringCanUI;
    public bool canUseWateringCan = true;

    private CharacterController characterController;
    private Vector3 velocity; // Current velocity for gravity and jumping

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        MovePlayer();
    }

    public void SelectWateringCan()
    {
        wateringCanUI.SetActive(true);
        wateringCan.SetActive(true);
    }

    public void DeselectWateringCan()
    {
        StopUsingWateringCan();
        wateringCanUI.SetActive(false);
        wateringCan.SetActive(false);
    }

    public void UseWateringCan()
    {
        if(GameManager.Instance.player.AvailableWater <= 0 || !canUseWateringCan) return;
        wateringCanAnimator.SetBool("IsWatering", true);
        GameManager.Instance.player.IsWatering = true;
    }

    public void StopUsingWateringCan()
    {
        wateringCanAnimator.SetBool("IsWatering", false);
        GameManager.Instance.player.IsWatering = false;
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

            if(Input.GetButtonDown("Jump") && GameManager.Instance.menuHistory.Count == 0)
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
