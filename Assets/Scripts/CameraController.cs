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
    public Slider mouseSenseSlider;

    public Transform playerBody; // Reference to the player's body to rotate with the camera
    public float mouseSensitivity = 2.0f; // Sensitivity for mouse movement

    private float xRotation = 0f; // Tracks vertical rotation (up/down)

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
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        UnityEngine.RaycastHit hit;
        bool hitObj = Physics.Raycast(ray, out hit, rayDistance, layersToCheck);
        Interactible i = hit.collider?.gameObject.GetComponent<Interactible>();

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

    //private void InteractECS()
    //{
    //    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //    PhysicsWorldSingleton physicsWorldSingleton = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>().Build(entityManager).GetSingleton<PhysicsWorldSingleton>();
    //    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

    //    float3 startPos = Camera.main.transform.position;
    //    float3 direction = Camera.main.transform.forward * 100;
    //    float3 endPos = startPos + direction;

    //    RaycastInput input = new()
    //    {
    //        Start = startPos,
    //        End = endPos,
    //        Filter = new CollisionFilter
    //        {
    //            BelongsTo = (uint)CollisionLayers.PlayerRay,
    //            CollidesWith = (uint)CollisionLayers.Enemy,
    //            GroupIndex = 0
    //        }
    //    };

    //    if(collisionWorld.CastRay(input, out Unity.Physics.RaycastHit rayCastHit))
    //    {
    //        if(entityManager.HasComponent<GrassComponent>(rayCastHit.Entity))
    //        {
    //            Debug.Log("Enemy hit in object code!");

    //            entityManager.GetComponentData<GrassComponent>(rayCastHit.Entity);
    //        }
    //    }
    //}
}
