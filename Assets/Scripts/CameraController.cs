using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region Singleton

    private static CameraController instance;
    public static CameraController Instance
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

    public Slider mouseSenseSlider;

    public Transform playerBody; // Reference to the player's body to rotate with the camera
    public float mouseSensitivity = 2.0f; // Sensitivity for mouse movement

    public bool canInteract = true;

    private float xRotation = 0f; // Tracks vertical rotation (up/down)

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen

        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);
        mouseSenseSlider.value = mouseSensitivity;
    }

    void Update()
    {
        if(GameManager.Instance.menuHistory.Count != 0)
        {
            return;
        }
        Interact();

        RotateCamera();
    }

    public void ChangeSensitivity(float sense)
    {
        mouseSensitivity = sense;
        Settings.Instance.SaveMouseSensitivity(sense);
    }

    private void RotateCamera()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 10 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 10 * Time.deltaTime;

        // Rotate the player body horizontally (yaw)
        playerBody?.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent over-rotation

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private Interactible lastObject = null;
    public LayerMask layersToCheck; // Variable for selected layers
    public float rayDistance = 10f; // Variable for ray distance
    private void Interact()
    {
        if(!canInteract && lastObject != null)
        {
            lastObject.OnHoverExit();
            lastObject = null;
            return;
        }
        else if(!canInteract) return;
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        UnityEngine.RaycastHit hit;
        bool hitObj = Physics.Raycast(ray, out hit, rayDistance, layersToCheck);
        Interactible i = hit.collider?.gameObject.GetComponentInParent<Interactible>();

        if(lastObject != null && ((i != null && lastObject != i) || i == null))
        {
            //Hovering nothing
            lastObject.OnHoverExit();
            lastObject = null;
        }
        if(hitObj)
        {
            //Hovering the object
            lastObject = i;
            lastObject.OnHoverEnter();
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && lastObject != null)
        {
            lastObject.onInteract();
        }
    }
}
