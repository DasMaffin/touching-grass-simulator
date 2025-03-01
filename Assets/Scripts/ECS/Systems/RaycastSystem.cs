using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public partial struct RaycastSystem : ISystem
{
    //private Entity _currentlySelectedEntity;
    //protected override void OnUpdate()
    //{
    //    // Get the physics collision world
    //    CollisionWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
    //    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    //    // Example: Player's position and forward direction
    //    float3 startPos = Camera.main.transform.position;
    //    float3 direction = Camera.main.transform.forward * 100;
    //    float3 endPos = startPos + direction;

    //    // Prepare the raycast input
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

    //    Entity hitEntity = Entity.Null;
    //    bool hitSomething = physicsWorld.CastRay(input, out Unity.Physics.RaycastHit hit);
    //    // Perform the raycast
    //    if(hitSomething)
    //    {
    //        // Get the hit entity
    //        hitEntity = hit.Entity;

    //        if(entityManager.HasComponent<SelectedEventComponent>(hitEntity))
    //        {
    //            var sec = entityManager.GetComponentData<SelectedEventComponent>(hitEntity);

    //            // If this entity wasn't selected before, set it as selected
    //            if(hitEntity != _currentlySelectedEntity)
    //            {
    //                if(_currentlySelectedEntity != Entity.Null &&
    //                    entityManager.HasComponent<SelectedEventComponent>(_currentlySelectedEntity))
    //                {
    //                    var prevSec = entityManager.GetComponentData<SelectedEventComponent>(_currentlySelectedEntity);
    //                    prevSec.onSelected = false;
    //                    prevSec.onDeselected = true;
    //                    prevSec.onInteract = false;
    //                    entityManager.SetComponentData(_currentlySelectedEntity, prevSec);
    //                }

    //                sec.onSelected = true;
    //                sec.onDeselected = false;
    //            }

    //            // Check if the player presses Mouse 1 (Fire1) for interaction
    //            if(Mouse.current.leftButton.wasPressedThisFrame)
    //            {
    //                sec.onInteract = true;
    //            }
    //            else
    //            {
    //                sec.onInteract = false;
    //            }

    //            entityManager.SetComponentData(hitEntity, sec);
    //            _currentlySelectedEntity = hitEntity;
    //        }
    //    }
    //    else
    //    {
    //        // If nothing was hit and there was a previously selected entity, deselect it
    //        if(_currentlySelectedEntity != Entity.Null &&
    //            entityManager.HasComponent<SelectedEventComponent>(_currentlySelectedEntity))
    //        {
    //            var sec = entityManager.GetComponentData<SelectedEventComponent>(_currentlySelectedEntity);
    //            sec.onDeselected = true;
    //            entityManager.SetComponentData(_currentlySelectedEntity, sec);
    //        }

    //        _currentlySelectedEntity = Entity.Null;
    //    }
    //}
}

//public static class CollisionLayers
//{
//    public const int PlayerRay = 1 << 0; // Layer for rays shot by the player
//    public const int Enemy = 1 << 8;    // Layer for enemy colliders
//}